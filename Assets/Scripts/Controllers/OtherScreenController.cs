using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using QonversionUnity;

namespace QonversionSample
{
    /// <summary>
    /// Controller for the Other screen.
    /// Contains miscellaneous SDK methods like sync, fallback check, platform-specific features.
    /// </summary>
    public class OtherScreenController : BaseScreenController
    {
        private Label _fallbackStatusLabel;
        private VisualElement _eligibilityContainer;

        public OtherScreenController(AppState appState) : base(appState) { }

        public override VisualElement CreateUI()
        {
            RootElement = CreateScrollView();

            var content = new VisualElement();
            content.style.paddingTop = 4;

            // Header
            var title = new Label("Other Methods");
            title.style.fontSize = 11;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = Color.white;
            title.style.marginBottom = 6;
            content.Add(title);

            // Fallback Section
            var fallbackSection = CreateSection("Fallback File");
            
            var fallbackButton = CreateButton("Check Fallback Accessibility", CheckFallbackFile);
            fallbackSection.Add(fallbackButton);

            var statusRow = new VisualElement();
            statusRow.style.flexDirection = FlexDirection.Row;
            statusRow.style.alignItems = Align.Center;
            statusRow.style.marginTop = 6;

            var statusLabel = new Label("Status:");
            statusLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            statusLabel.style.marginRight = 8;
            statusLabel.style.fontSize = 9;
            statusRow.Add(statusLabel);

            _fallbackStatusLabel = new Label("Not checked");
            _fallbackStatusLabel.style.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            _fallbackStatusLabel.style.fontSize = 9;
            statusRow.Add(_fallbackStatusLabel);

            fallbackSection.Add(statusRow);
            content.Add(fallbackSection);

            // Sync Section
            var syncSection = CreateSection("Sync");
            
            var syncHistoricalButton = CreateButton("Sync Historical Data", SyncHistoricalData);
            syncSection.Add(syncHistoricalButton);

            syncSection.Add(CreatePlatformNote("Syncs subscriber data. Useful during initial SDK implementation."));

            content.Add(syncSection);

            // iOS Only Section
            var iosSection = CreateSection("iOS Only");
            
            var advertIdButton = CreateButton("Collect Advertising ID", CollectAdvertisingId);
            iosSection.Add(advertIdButton);

            var searchAdsButton = CreateButton("Collect Apple Search Ads Attribution", CollectAppleSearchAdsAttribution);
            iosSection.Add(searchAdsButton);

            var codeRedemptionButton = CreateButton("Present Code Redemption Sheet", PresentCodeRedemptionSheet);
            iosSection.Add(codeRedemptionButton);

            var storeKit2Button = CreateButton("Sync StoreKit 2 Purchases", SyncStoreKit2Purchases, false);
            iosSection.Add(storeKit2Button);

            iosSection.Add(CreatePlatformNote("These methods only work on iOS. On other platforms, they will have no effect."));

            content.Add(iosSection);

            // Android Only Section
            var androidSection = CreateSection("Android Only");
            
            var syncPurchasesButton = CreateButton("Sync Purchases", SyncPurchases);
            androidSection.Add(syncPurchasesButton);

            androidSection.Add(CreatePlatformNote("Only for Android in Observer mode. Syncs purchases handled by custom implementation."));

            content.Add(androidSection);

            // Eligibility Section
            var eligibilitySection = CreateSection("Trial/Intro Eligibility");
            
            var checkEligibilityButton = CreateButton("Check Eligibility", CheckEligibility);
            eligibilitySection.Add(checkEligibilityButton);

            _eligibilityContainer = new VisualElement();
            _eligibilityContainer.style.marginTop = 6;
            eligibilitySection.Add(_eligibilityContainer);

            content.Add(eligibilitySection);

            // Experiment Attachment Section (for testing)
            var experimentSection = CreateSection("Experiment Attachment (Testing)");
            experimentSection.Add(CreatePlatformNote("⚠️ For testing purposes only. Remove before release."));
            
            var attachExperimentRow = CreateInputButtonRow("Experiment ID:", "Attach", AttachToExperiment);
            experimentSection.Add(attachExperimentRow);

            content.Add(experimentSection);

            ((ScrollView)RootElement).Add(content);

            Refresh();
            return RootElement;
        }

        public override void Refresh()
        {
            UpdateEligibilities();
        }

        private VisualElement CreatePlatformNote(string text)
        {
            var note = new Label(text);
            note.style.color = new Color(0.55f, 0.55f, 0.6f, 1f);
            note.style.fontSize = 8;
            note.style.unityFontStyleAndWeight = FontStyle.Italic;
            note.style.marginTop = 4;
            note.style.whiteSpace = WhiteSpace.Normal;
            return note;
        }

        private VisualElement CreateInputButtonRow(string labelText, string buttonText, System.Action<string> onButtonClick)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.marginTop = 6;
            row.style.flexShrink = 1;
            row.style.overflow = Overflow.Hidden;

            var label = new Label(labelText);
            label.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            label.style.width = 70;
            label.style.fontSize = 9;
            label.style.flexShrink = 0;
            row.Add(label);

            var textField = new TextField();
            textField.style.flexGrow = 1;
            textField.style.marginRight = 6;
            textField.style.fontSize = 10;
            textField.style.minHeight = 28;
            textField.style.backgroundColor = new Color(0.15f, 0.15f, 0.18f, 1f);
            textField.style.borderTopLeftRadius = 4;
            textField.style.borderTopRightRadius = 4;
            textField.style.borderBottomLeftRadius = 4;
            textField.style.borderBottomRightRadius = 4;
            textField.style.paddingLeft = 6;
            textField.style.paddingRight = 6;
            row.Add(textField);

            var button = CreateButton(buttonText, () => onButtonClick(textField.value));
            button.style.height = 28;
            row.Add(button);

            return row;
        }

        private void UpdateEligibilities()
        {
            if (_eligibilityContainer == null) return;
            _eligibilityContainer.Clear();

            var eligibilities = AppState.Eligibilities;
            if (eligibilities == null || eligibilities.Count == 0)
            {
                var emptyLabel = new Label("Click 'Check Eligibility' to check products.");
                emptyLabel.style.color = new Color(0.55f, 0.55f, 0.6f, 1f);
                emptyLabel.style.fontSize = 8;
                emptyLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
                emptyLabel.style.whiteSpace = WhiteSpace.Normal;
                _eligibilityContainer.Add(emptyLabel);
                return;
            }

            foreach (var kvp in eligibilities)
            {
                var row = CreateInfoRow(kvp.Key, kvp.Value.Status.ToString());
                _eligibilityContainer.Add(row);
            }
        }

        private void CheckFallbackFile()
        {
            Debug.Log("🔄 [Qonversion] Checking fallback file accessibility...");
            AppState.SetLoading(true);

            Qonversion.GetSharedInstance().IsFallbackFileAccessible((accessible) =>
            {
                AppState.SetLoading(false);

                Debug.Log($"✅ [Qonversion] Fallback accessible: {accessible}");
                
                if (accessible)
                {
                    _fallbackStatusLabel.text = "✓ Accessible";
                    _fallbackStatusLabel.style.color = new Color(0.2f, 0.8f, 0.3f, 1f);
                }
                else
                {
                    _fallbackStatusLabel.text = "✗ Not accessible";
                    _fallbackStatusLabel.style.color = new Color(0.9f, 0.2f, 0.2f, 1f);
                }

                AppState.ShowSuccess($"Fallback file accessible: {accessible}");
            });
        }

        private void SyncHistoricalData()
        {
            Debug.Log("🔄 [Qonversion] Syncing historical data...");

            try
            {
                Qonversion.GetSharedInstance().SyncHistoricalData();
                Debug.Log("✅ [Qonversion] Historical data sync initiated");
                AppState.ShowSuccess("Historical data sync initiated");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ [Qonversion] Failed to sync historical data: {e.Message}");
                AppState.ShowError($"Failed to sync: {e.Message}");
            }
        }

        private void CollectAdvertisingId()
        {
            Debug.Log("🔄 [Qonversion] Collecting advertising ID...");

            try
            {
                Qonversion.GetSharedInstance().CollectAdvertisingId();
                Debug.Log("✅ [Qonversion] Advertising ID collection initiated");
                AppState.ShowSuccess("Advertising ID collection initiated (iOS only)");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ [Qonversion] Failed to collect advertising ID: {e.Message}");
                AppState.ShowError($"Failed: {e.Message}");
            }
        }

        private void CollectAppleSearchAdsAttribution()
        {
            Debug.Log("🔄 [Qonversion] Collecting Apple Search Ads attribution...");

            try
            {
                Qonversion.GetSharedInstance().CollectAppleSearchAdsAttribution();
                Debug.Log("✅ [Qonversion] Apple Search Ads attribution collection initiated");
                AppState.ShowSuccess("Apple Search Ads attribution initiated (iOS only)");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ [Qonversion] Failed: {e.Message}");
                AppState.ShowError($"Failed: {e.Message}");
            }
        }

        private void PresentCodeRedemptionSheet()
        {
            Debug.Log("🔄 [Qonversion] Presenting code redemption sheet...");

            try
            {
                Qonversion.GetSharedInstance().PresentCodeRedemptionSheet();
                Debug.Log("✅ [Qonversion] Code redemption sheet presented");
                AppState.ShowSuccess("Code redemption sheet presented (iOS 14+ only)");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ [Qonversion] Failed: {e.Message}");
                AppState.ShowError($"Failed: {e.Message}");
            }
        }

        private void SyncStoreKit2Purchases()
        {
            Debug.Log("🔄 [Qonversion] Syncing StoreKit 2 purchases...");

            try
            {
                Qonversion.GetSharedInstance().SyncStoreKit2Purchases();
                Debug.Log("✅ [Qonversion] StoreKit 2 purchases sync initiated");
                AppState.ShowSuccess("StoreKit 2 purchases sync initiated (iOS only)");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ [Qonversion] Failed: {e.Message}");
                AppState.ShowError($"Failed: {e.Message}");
            }
        }

        private void SyncPurchases()
        {
            Debug.Log("🔄 [Qonversion] Syncing purchases...");

            try
            {
                Qonversion.GetSharedInstance().SyncPurchases();
                Debug.Log("✅ [Qonversion] Purchases sync initiated");
                AppState.ShowSuccess("Purchases sync initiated (Android only)");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ [Qonversion] Failed: {e.Message}");
                AppState.ShowError($"Failed: {e.Message}");
            }
        }

        private void CheckEligibility()
        {
            var products = AppState.Products;
            if (products == null || products.Count == 0)
            {
                AppState.ShowError("Please load products first");
                return;
            }

            Debug.Log("🔄 [Qonversion] Checking trial/intro eligibility...");
            AppState.SetLoading(true);

            var productIds = new List<string>(products.Keys);

            Qonversion.GetSharedInstance().CheckTrialIntroEligibility(productIds, (eligibilities, error) =>
            {
                AppState.SetLoading(false);

                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to check eligibility: {error.Message}");
                    AppState.ShowError($"Failed to check eligibility: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] Eligibility checked for {eligibilities?.Count ?? 0} products");
                AppState.SetEligibilities(eligibilities);
                AppState.ShowSuccess($"Eligibility checked for {eligibilities?.Count ?? 0} products");
            });
        }

        private void AttachToExperiment(string experimentId)
        {
            if (string.IsNullOrEmpty(experimentId))
            {
                AppState.ShowError("Please enter an experiment ID");
                return;
            }

            Debug.Log($"🔄 [Qonversion] Attaching to experiment: {experimentId}...");
            AppState.SetLoading(true);

            // Using a default group ID for testing
            Qonversion.GetSharedInstance().AttachUserToExperiment(experimentId, "control", (success, error) =>
            {
                AppState.SetLoading(false);

                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to attach to experiment: {error.Message}");
                    AppState.ShowError($"Failed to attach: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] Attached to experiment: {experimentId}");
                AppState.ShowSuccess($"Attached to experiment: {experimentId}");
            });
        }
    }
}
