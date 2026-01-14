using UnityEngine;
using QonversionUnity;

namespace QonversionSample
{
    /// <summary>
    /// Main entry point for the Qonversion SDK Sample App.
    /// Initializes the SDK and sets up the UI.
    /// </summary>
    public class SampleApp : MonoBehaviour, NoCodesDelegate, NoCodesPurchaseDelegate
    {
        private const string ProjectKey = "PV77YHL7qnGvsdmpTs7gimsxUvY-Znl2";

        [SerializeField] private UIManager uiManager;

        private AppState _appState;

        private void Awake()
        {
            _appState = new AppState();
            
            if (uiManager != null)
            {
                uiManager.Initialize(_appState);
            }
        }

        private void Start()
        {
            InitializeQonversion();
            InitializeNoCodes();
        }

        private void InitializeQonversion()
        {
            Debug.Log("🔄 [Qonversion] Starting SDK initialization...");
            _appState.SetInitStatus(InitStatus.Initializing);

            try
            {
                var config = new QonversionConfigBuilder(ProjectKey, LaunchMode.SubscriptionManagement)
                    .SetEnvironment(Environment.Sandbox)
                    .SetEntitlementsCacheLifetime(EntitlementsCacheLifetime.Month)
                    .Build();

                Qonversion.Initialize(config);
                
                // Subscribe to events
                var instance = Qonversion.GetSharedInstance();
                instance.UpdatedEntitlementsReceived += OnUpdatedEntitlementsReceived;
                instance.PromoPurchasesReceived += OnPromoPurchasesReceived;

                Debug.Log("✅ [Qonversion] SDK initialized successfully");
                _appState.SetInitStatus(InitStatus.Success);

                // Load initial user info
                LoadUserInfo();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ [Qonversion] SDK initialization failed: {e.Message}");
                _appState.SetInitStatus(InitStatus.Error);
            }
        }

        private void LoadUserInfo()
        {
            Debug.Log("🔄 [Qonversion] Loading user info...");
            Qonversion.GetSharedInstance().UserInfo((user, error) =>
            {
                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to load user info: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] User info loaded: {user.QonversionId}");
                _appState.SetUserInfo(user);
            });
        }

        private void OnUpdatedEntitlementsReceived(System.Collections.Generic.Dictionary<string, Entitlement> entitlements)
        {
            Debug.Log($"📡 [Qonversion] Entitlements updated: {entitlements.Count} entitlements");
            _appState.SetEntitlements(entitlements);
        }

        private void OnPromoPurchasesReceived(string productId, Qonversion.StartPromoPurchase purchaseDelegate)
        {
            Debug.Log($"📡 [Qonversion] Promo purchase received for product: {productId}");
            // Auto-proceed with the promo purchase
            purchaseDelegate((entitlements, error) =>
            {
                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Promo purchase failed: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] Promo purchase completed: {entitlements.Count} entitlements");
                _appState.SetEntitlements(entitlements);
            });
        }

        private void InitializeNoCodes()
        {
            Debug.Log("🔄 [NoCodes] Starting No-Codes SDK initialization...");

            try
            {
                // Initialize without purchase delegate - it can be enabled later
                var noCodesConfig = new NoCodesConfigBuilder(ProjectKey)
                    .SetNoCodesDelegate(this)
                    .Build();

                NoCodes.Initialize(noCodesConfig);

                Debug.Log("✅ [NoCodes] SDK initialized successfully");
                _appState.SetNoCodesInitialized(true);
                _appState.AddNoCodesEvent("✅ No-Codes SDK initialized (default purchase flow)");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ [NoCodes] SDK initialization failed: {e.Message}");
                _appState.AddNoCodesEvent($"❌ Initialization failed: {e.Message}");
            }
        }

        /// <summary>
        /// Enables custom purchase delegate for No-Codes.
        /// Called from NoCodesScreenController when user clicks the enable button.
        /// </summary>
        public void EnableCustomPurchaseDelegate()
        {
            if (!_appState.NoCodesInitialized)
            {
                _appState.ShowError("No-Codes is not initialized");
                return;
            }

            if (_appState.NoCodesPurchaseDelegateEnabled)
            {
                _appState.ShowSuccess("Custom purchase delegate is already enabled");
                return;
            }

            try
            {
                Debug.Log("🔄 [NoCodes] Enabling custom purchase delegate...");
                NoCodes.GetSharedInstance().SetPurchaseDelegate(this);
                _appState.SetNoCodesPurchaseDelegateEnabled(true);
                _appState.AddNoCodesEvent("✅ Custom purchase delegate ENABLED");
                _appState.ShowSuccess("Custom purchase delegate enabled!");
                Debug.Log("✅ [NoCodes] Custom purchase delegate enabled");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ [NoCodes] Failed to enable purchase delegate: {e.Message}");
                _appState.ShowError($"Failed to enable: {e.Message}");
            }
        }

        #region NoCodesDelegate Implementation

        public void OnScreenShown(string screenId)
        {
            Debug.Log($"📱 [NoCodes] Screen shown: {screenId}");
            _appState.AddNoCodesEvent($"📱 Screen shown: {screenId}");
        }

        public void OnActionStarted(NoCodesAction action)
        {
            Debug.Log($"▶️ [NoCodes] Action started: {action.Type}");
            _appState.AddNoCodesEvent($"▶️ Action started: {action.Type}");
        }

        public void OnActionFailed(NoCodesAction action)
        {
            var errorMsg = action.Error?.Description ?? "Unknown error";
            Debug.Log($"❌ [NoCodes] Action failed: {action.Type} - {errorMsg}");
            _appState.AddNoCodesEvent($"❌ Action failed: {action.Type} - {errorMsg}");
        }

        public void OnActionFinished(NoCodesAction action)
        {
            Debug.Log($"✅ [NoCodes] Action finished: {action.Type}");
            _appState.AddNoCodesEvent($"✅ Action finished: {action.Type}");
        }

        public void OnFinished()
        {
            Debug.Log("🏁 [NoCodes] Flow finished");
            _appState.AddNoCodesEvent("🏁 No-Codes flow finished");
        }

        public void OnScreenFailedToLoad(NoCodesError error)
        {
            var errorMsg = error.Description ?? "Unknown error";
            Debug.LogError($"❌ [NoCodes] Screen failed to load: {errorMsg}");
            _appState.AddNoCodesEvent($"❌ Screen failed to load: {errorMsg}");
            _appState.ShowError($"Screen failed to load: {errorMsg}");
        }

        #endregion

        #region NoCodesPurchaseDelegate Implementation

        public void Purchase(Product product, System.Action onSuccess, System.Action<string> onError)
        {
            Debug.Log($"🛒 [NoCodes] Purchase delegate called for: {product.QonversionId}");
            _appState.AddNoCodesEvent($"🛒 Purchase requested: {product.QonversionId}");

            // Use Qonversion SDK to perform the purchase
            Qonversion.GetSharedInstance().PurchaseProduct(product, (entitlements, error, isCancelled) =>
            {
                if (isCancelled)
                {
                    Debug.Log("⚠️ [NoCodes] Purchase cancelled by user");
                    _appState.AddNoCodesEvent("⚠️ Purchase cancelled by user");
                    onError?.Invoke("Purchase cancelled by user");
                    return;
                }

                if (error != null)
                {
                    Debug.LogError($"❌ [NoCodes] Purchase failed: {error.Message}");
                    _appState.AddNoCodesEvent($"❌ Purchase failed: {error.Message}");
                    onError?.Invoke(error.Message);
                    return;
                }

                Debug.Log($"✅ [NoCodes] Purchase successful: {entitlements.Count} entitlements");
                _appState.AddNoCodesEvent($"✅ Purchase successful!");
                _appState.SetEntitlements(entitlements);
                onSuccess?.Invoke();
            });
        }

        public void Restore(System.Action onSuccess, System.Action<string> onError)
        {
            Debug.Log("🔄 [NoCodes] Restore delegate called");
            _appState.AddNoCodesEvent("🔄 Restore requested");

            // Use Qonversion SDK to restore purchases
            Qonversion.GetSharedInstance().Restore((entitlements, error) =>
            {
                if (error != null)
                {
                    Debug.LogError($"❌ [NoCodes] Restore failed: {error.Message}");
                    _appState.AddNoCodesEvent($"❌ Restore failed: {error.Message}");
                    onError?.Invoke(error.Message);
                    return;
                }

                Debug.Log($"✅ [NoCodes] Restore successful: {entitlements.Count} entitlements");
                _appState.AddNoCodesEvent($"✅ Restore successful: {entitlements.Count} entitlements");
                _appState.SetEntitlements(entitlements);
                onSuccess?.Invoke();
            });
        }

        #endregion

        private void OnDestroy()
        {
            var instance = Qonversion.GetSharedInstance();
            if (instance != null)
            {
                instance.UpdatedEntitlementsReceived -= OnUpdatedEntitlementsReceived;
                instance.PromoPurchasesReceived -= OnPromoPurchasesReceived;
            }
        }
    }
}
