using UnityEngine;
using UnityEngine.UIElements;
using QonversionUnity;

namespace QonversionSample
{
    /// <summary>
    /// Controller for the main/home screen.
    /// Shows SDK status, user info, and quick actions.
    /// </summary>
    public class MainScreenController : BaseScreenController
    {
        private Label _statusLabel;
        private VisualElement _userInfoContainer;
        private VisualElement _quickActionsContainer;

        public MainScreenController(AppState appState) : base(appState) { }

        public override VisualElement CreateUI()
        {
            RootElement = CreateScrollView();

            var content = new VisualElement();
            content.style.paddingTop = 4;

            // Title
            var title = new Label("Qonversion SDK Demo");
            title.style.fontSize = 12;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.unityTextAlign = TextAnchor.MiddleCenter;
            title.style.marginBottom = 8;
            title.style.color = Color.white;
            content.Add(title);

            // SDK Status Section
            var statusSection = CreateSection("SDK Status");
            _statusLabel = new Label();
            _statusLabel.style.fontSize = 9;
            statusSection.Add(_statusLabel);
            content.Add(statusSection);

            // User Info Section
            var userSection = CreateSection("Current User");
            _userInfoContainer = new VisualElement();
            userSection.Add(_userInfoContainer);
            content.Add(userSection);

            // Quick Actions Section
            var actionsSection = CreateSection("Quick Actions");
            _quickActionsContainer = new VisualElement();
            _quickActionsContainer.style.flexDirection = FlexDirection.Row;
            _quickActionsContainer.style.flexWrap = Wrap.Wrap;
            
            var actions = new (string label, System.Action action, Color color)[]
            {
                ("Load Products", LoadProducts, new Color(0.2f, 0.5f, 0.9f, 1f)),
                ("Check Entitlements", CheckEntitlements, new Color(0.2f, 0.7f, 0.3f, 1f)),
                ("Load Offerings", LoadOfferings, new Color(0.6f, 0.3f, 0.8f, 1f)),
                ("Restore Purchases", RestorePurchases, new Color(0.9f, 0.5f, 0.2f, 1f)),
                ("Load Remote Configs", LoadRemoteConfigs, new Color(0.8f, 0.2f, 0.5f, 1f)),
                ("Reload User Info", ReloadUserInfo, new Color(0.3f, 0.6f, 0.8f, 1f))
            };

            foreach (var (label, action, color) in actions)
            {
                var button = CreateQuickActionButton(label, action, color);
                _quickActionsContainer.Add(button);
            }

            actionsSection.Add(_quickActionsContainer);
            content.Add(actionsSection);

            ((ScrollView)RootElement).Add(content);

            Refresh();
            return RootElement;
        }

        private VisualElement CreateQuickActionButton(string label, System.Action action, Color color)
        {
            var button = new Button(action);
            button.style.width = 80;
            button.style.height = 40;
            button.style.marginRight = 4;
            button.style.marginBottom = 4;
            button.style.backgroundColor = color;
            button.style.borderTopLeftRadius = 6;
            button.style.borderTopRightRadius = 6;
            button.style.borderBottomLeftRadius = 6;
            button.style.borderBottomRightRadius = 6;
            button.style.justifyContent = Justify.Center;
            button.style.alignItems = Align.Center;

            var buttonLabel = new Label(label);
            buttonLabel.style.color = Color.white;
            buttonLabel.style.fontSize = 7;
            buttonLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            buttonLabel.style.whiteSpace = WhiteSpace.Normal;
            button.Add(buttonLabel);

            return button;
        }

        public override void Refresh()
        {
            UpdateStatusLabel();
            UpdateUserInfo();
        }

        private void UpdateStatusLabel()
        {
            if (_statusLabel == null) return;

            string statusText;
            Color statusColor;

            switch (AppState.InitStatus)
            {
                case InitStatus.NotInitialized:
                    statusText = "⚪ Not Initialized";
                    statusColor = Color.gray;
                    break;
                case InitStatus.Initializing:
                    statusText = "🔄 Initializing...";
                    statusColor = Color.yellow;
                    break;
                case InitStatus.Success:
                    statusText = "✅ SDK Initialized Successfully";
                    statusColor = new Color(0.2f, 0.8f, 0.3f, 1f);
                    break;
                case InitStatus.Error:
                    statusText = "❌ Initialization Failed";
                    statusColor = new Color(0.9f, 0.2f, 0.2f, 1f);
                    break;
                default:
                    statusText = "Unknown";
                    statusColor = Color.gray;
                    break;
            }

            _statusLabel.text = statusText;
            _statusLabel.style.color = statusColor;
        }

        private void UpdateUserInfo()
        {
            if (_userInfoContainer == null) return;
            _userInfoContainer.Clear();

            var user = AppState.UserInfo;
            if (user == null)
            {
                var noUserLabel = new Label("No user info loaded");
                noUserLabel.style.color = new Color(0.6f, 0.6f, 0.6f, 1f);
                noUserLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
                _userInfoContainer.Add(noUserLabel);
                return;
            }

            _userInfoContainer.Add(CreateInfoRow("Qonversion ID", user.QonversionId));
            _userInfoContainer.Add(CreateInfoRow("Identity ID", user.IdentityId ?? "Anonymous"));
        }

        private void LoadProducts()
        {
            Debug.Log("🔄 [Qonversion] Loading products...");
            AppState.SetLoading(true);

            Qonversion.GetSharedInstance().Products((products, error) =>
            {
                AppState.SetLoading(false);

                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to load products: {error.Message}");
                    AppState.ShowError($"Failed to load products: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] Products loaded: {products.Count}");
                AppState.SetProducts(products);
                AppState.ShowSuccess($"Loaded {products.Count} products");
            });
        }

        private void CheckEntitlements()
        {
            Debug.Log("🔄 [Qonversion] Checking entitlements...");
            AppState.SetLoading(true);

            Qonversion.GetSharedInstance().CheckEntitlements((entitlements, error) =>
            {
                AppState.SetLoading(false);

                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to check entitlements: {error.Message}");
                    AppState.ShowError($"Failed to check entitlements: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] Entitlements checked: {entitlements.Count}");
                AppState.SetEntitlements(entitlements);
                AppState.ShowSuccess($"Found {entitlements.Count} entitlements");
            });
        }

        private void LoadOfferings()
        {
            Debug.Log("🔄 [Qonversion] Loading offerings...");
            AppState.SetLoading(true);

            Qonversion.GetSharedInstance().Offerings((offerings, error) =>
            {
                AppState.SetLoading(false);

                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to load offerings: {error.Message}");
                    AppState.ShowError($"Failed to load offerings: {error.Message}");
                    return;
                }

                Debug.Log("✅ [Qonversion] Offerings loaded");
                AppState.SetOfferings(offerings);
                AppState.ShowSuccess("Offerings loaded successfully");
            });
        }

        private void RestorePurchases()
        {
            Debug.Log("🔄 [Qonversion] Restoring purchases...");
            AppState.SetLoading(true);

            Qonversion.GetSharedInstance().Restore((entitlements, error) =>
            {
                AppState.SetLoading(false);

                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to restore purchases: {error.Message}");
                    AppState.ShowError($"Failed to restore: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] Purchases restored: {entitlements.Count} entitlements");
                AppState.SetEntitlements(entitlements);
                AppState.ShowSuccess($"Restored {entitlements.Count} entitlements");
            });
        }

        private void LoadRemoteConfigs()
        {
            Debug.Log("🔄 [Qonversion] Loading remote configs...");
            AppState.SetLoading(true);

            Qonversion.GetSharedInstance().RemoteConfigList((configList, error) =>
            {
                AppState.SetLoading(false);

                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to load remote configs: {error.Message}");
                    AppState.ShowError($"Failed to load configs: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] Remote configs loaded: {configList?.RemoteConfigs?.Count ?? 0}");
                AppState.SetRemoteConfigList(configList);
                AppState.ShowSuccess("Remote configs loaded");
            });
        }

        private void ReloadUserInfo()
        {
            Debug.Log("🔄 [Qonversion] Reloading user info...");
            AppState.SetLoading(true);

            Qonversion.GetSharedInstance().UserInfo((user, error) =>
            {
                AppState.SetLoading(false);

                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to load user info: {error.Message}");
                    AppState.ShowError($"Failed to load user info: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] User info loaded: {user.QonversionId}");
                AppState.SetUserInfo(user);
                AppState.ShowSuccess("User info reloaded");
            });
        }
    }
}
