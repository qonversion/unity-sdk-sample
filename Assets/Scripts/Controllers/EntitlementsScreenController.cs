using UnityEngine;
using UnityEngine.UIElements;
using QonversionUnity;

namespace QonversionSample
{
    /// <summary>
    /// Controller for the Entitlements screen.
    /// Displays entitlements and allows restore purchases.
    /// </summary>
    public class EntitlementsScreenController : BaseScreenController
    {
        private VisualElement _entitlementsContainer;
        private Label _emptyLabel;

        public EntitlementsScreenController(AppState appState) : base(appState) { }

        public override VisualElement CreateUI()
        {
            RootElement = new VisualElement();
            RootElement.style.flexGrow = 1;

            // Header with buttons
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.justifyContent = Justify.SpaceBetween;
            header.style.alignItems = Align.Center;
            header.style.marginBottom = 6;

            var title = new Label("Entitlements");
            title.style.fontSize = 11;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = Color.white;
            header.Add(title);

            var buttonsContainer = new VisualElement();
            buttonsContainer.style.flexDirection = FlexDirection.Row;

            var checkButton = CreateButton("Check", CheckEntitlements);
            checkButton.style.marginRight = 3;
            buttonsContainer.Add(checkButton);

            var restoreButton = CreateButton("Restore", RestorePurchases, false);
            buttonsContainer.Add(restoreButton);

            header.Add(buttonsContainer);
            RootElement.Add(header);

            // Empty state label
            _emptyLabel = new Label("No entitlements. Click 'Check'.");
            _emptyLabel.style.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            _emptyLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
            _emptyLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _emptyLabel.style.marginTop = 16;
            _emptyLabel.style.fontSize = 8;
            RootElement.Add(_emptyLabel);

            // Entitlements container (scrollable)
            var scrollView = CreateScrollView();
            _entitlementsContainer = new VisualElement();
            scrollView.Add(_entitlementsContainer);
            RootElement.Add(scrollView);

            Refresh();
            return RootElement;
        }

        public override void Refresh()
        {
            if (_entitlementsContainer == null) return;
            _entitlementsContainer.Clear();

            var entitlements = AppState.Entitlements;

            if (entitlements == null)
            {
                _emptyLabel.text = "No entitlements checked yet. Click 'Check' to verify entitlements.";
                _emptyLabel.style.display = DisplayStyle.Flex;
                return;
            }

            if (entitlements.Count == 0)
            {
                _emptyLabel.text = "No active entitlements found.";
                _emptyLabel.style.display = DisplayStyle.Flex;
                return;
            }

            _emptyLabel.style.display = DisplayStyle.None;

            foreach (var kvp in entitlements)
            {
                var entitlement = kvp.Value;
                var card = CreateEntitlementCard(entitlement);
                _entitlementsContainer.Add(card);
            }
        }

        private VisualElement CreateEntitlementCard(Entitlement entitlement)
        {
            var card = CreateCard();

            // Header row with status
            var headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            headerRow.style.justifyContent = Justify.SpaceBetween;
            headerRow.style.alignItems = Align.Center;
            headerRow.style.marginBottom = 12;

            // Left side: icon + info
            var leftContainer = new VisualElement();
            leftContainer.style.flexDirection = FlexDirection.Row;
            leftContainer.style.alignItems = Align.Center;

            // Status icon
            var iconColor = entitlement.IsActive ? new Color(0.2f, 0.8f, 0.3f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f);
            var iconLabel = new Label(entitlement.IsActive ? "✓" : "✗");
            iconLabel.style.width = 32;
            iconLabel.style.height = 32;
            iconLabel.style.backgroundColor = iconColor;
            iconLabel.style.color = Color.white;
            iconLabel.style.borderTopLeftRadius = 8;
            iconLabel.style.borderTopRightRadius = 8;
            iconLabel.style.borderBottomLeftRadius = 8;
            iconLabel.style.borderBottomRightRadius = 8;
            iconLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            iconLabel.style.fontSize = 18;
            iconLabel.style.marginRight = 12;
            leftContainer.Add(iconLabel);

            // Info
            var infoContainer = new VisualElement();

            var titleLabel = new Label(entitlement.Id);
            titleLabel.style.fontSize = 16;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.color = Color.white;
            infoContainer.Add(titleLabel);

            var productLabel = new Label($"Product: {entitlement.ProductId}");
            productLabel.style.fontSize = 12;
            productLabel.style.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            productLabel.style.marginTop = 2;
            infoContainer.Add(productLabel);

            leftContainer.Add(infoContainer);
            headerRow.Add(leftContainer);

            // Status badge
            var statusColor = entitlement.IsActive ? new Color(0.2f, 0.7f, 0.3f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f);
            var statusChip = CreateChip(entitlement.IsActive ? "Active" : "Inactive", statusColor);
            headerRow.Add(statusChip);

            card.Add(headerRow);

            // Divider
            var divider = new VisualElement();
            divider.style.height = 1;
            divider.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            divider.style.marginTop = 8;
            divider.style.marginBottom = 12;
            card.Add(divider);

            // Details
            card.Add(CreateInfoRow("Renew State", entitlement.RenewState.ToString()));
            card.Add(CreateInfoRow("Source", entitlement.Source.ToString()));

            card.Add(CreateInfoRow("Started", FormatDate(entitlement.StartedDate)));

            if (entitlement.ExpirationDate.HasValue)
            {
                card.Add(CreateInfoRow("Expires", FormatDate(entitlement.ExpirationDate.Value)));
            }

            return card;
        }

        private string FormatDate(System.DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm");
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
    }
}
