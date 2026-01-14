using UnityEngine;
using UnityEngine.UIElements;
using QonversionUnity;

namespace QonversionSample
{
    /// <summary>
    /// Controller for the Offerings screen.
    /// Displays offerings with their products.
    /// </summary>
    public class OfferingsScreenController : BaseScreenController
    {
        private VisualElement _offeringsContainer;
        private Label _emptyLabel;

        public OfferingsScreenController(AppState appState) : base(appState) { }

        public override VisualElement CreateUI()
        {
            RootElement = new VisualElement();
            RootElement.style.flexGrow = 1;

            // Header
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.justifyContent = Justify.SpaceBetween;
            header.style.alignItems = Align.Center;
            header.style.marginBottom = 6;

            var title = new Label("Offerings");
            title.style.fontSize = 11;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = Color.white;
            header.Add(title);

            var reloadButton = CreateButton("Reload", LoadOfferings);
            header.Add(reloadButton);

            RootElement.Add(header);

            // Empty state
            _emptyLabel = new Label("No offerings. Click 'Reload'.");
            _emptyLabel.style.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            _emptyLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
            _emptyLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _emptyLabel.style.marginTop = 16;
            _emptyLabel.style.fontSize = 8;
            RootElement.Add(_emptyLabel);

            // Offerings container
            var scrollView = CreateScrollView();
            _offeringsContainer = new VisualElement();
            scrollView.Add(_offeringsContainer);
            RootElement.Add(scrollView);

            Refresh();
            return RootElement;
        }

        public override void Refresh()
        {
            if (_offeringsContainer == null) return;
            _offeringsContainer.Clear();

            var offerings = AppState.Offerings;

            if (offerings == null)
            {
                _emptyLabel.style.display = DisplayStyle.Flex;
                return;
            }

            var allOfferings = new System.Collections.Generic.List<Offering>();
            if (offerings.Main != null) allOfferings.Add(offerings.Main);
            if (offerings.AvailableOfferings != null)
            {
                foreach (var offering in offerings.AvailableOfferings)
                {
                    if (offerings.Main == null || offering.Id != offerings.Main.Id)
                    {
                        allOfferings.Add(offering);
                    }
                }
            }

            if (allOfferings.Count == 0)
            {
                _emptyLabel.text = "No offerings available.";
                _emptyLabel.style.display = DisplayStyle.Flex;
                return;
            }

            _emptyLabel.style.display = DisplayStyle.None;

            foreach (var offering in allOfferings)
            {
                var isMain = offerings.Main != null && offering.Id == offerings.Main.Id;
                var card = CreateOfferingCard(offering, isMain);
                _offeringsContainer.Add(card);
            }
        }

        private VisualElement CreateOfferingCard(Offering offering, bool isMain)
        {
            var card = CreateCard();

            // Header
            var headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            headerRow.style.justifyContent = Justify.SpaceBetween;
            headerRow.style.alignItems = Align.Center;
            headerRow.style.marginBottom = 12;

            // Icon + info
            var leftContainer = new VisualElement();
            leftContainer.style.flexDirection = FlexDirection.Row;
            leftContainer.style.alignItems = Align.Center;

            var iconColor = isMain ? new Color(0.2f, 0.5f, 0.9f, 1f) : new Color(0.9f, 0.5f, 0.2f, 1f);
            var iconLabel = new Label(isMain ? "★" : "◆");
            iconLabel.style.width = 36;
            iconLabel.style.height = 36;
            iconLabel.style.backgroundColor = iconColor;
            iconLabel.style.color = Color.white;
            iconLabel.style.borderTopLeftRadius = 10;
            iconLabel.style.borderTopRightRadius = 10;
            iconLabel.style.borderBottomLeftRadius = 10;
            iconLabel.style.borderBottomRightRadius = 10;
            iconLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            iconLabel.style.fontSize = 18;
            iconLabel.style.marginRight = 12;
            leftContainer.Add(iconLabel);

            var infoContainer = new VisualElement();

            var titleRow = new VisualElement();
            titleRow.style.flexDirection = FlexDirection.Row;
            titleRow.style.alignItems = Align.Center;

            var titleLabel = new Label(offering.Id);
            titleLabel.style.fontSize = 16;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.color = Color.white;
            titleRow.Add(titleLabel);

            if (isMain)
            {
                var mainBadge = new Label("MAIN");
                mainBadge.style.backgroundColor = new Color(0.2f, 0.5f, 0.9f, 1f);
                mainBadge.style.color = Color.white;
                mainBadge.style.paddingTop = 2;
                mainBadge.style.paddingBottom = 2;
                mainBadge.style.paddingLeft = 8;
                mainBadge.style.paddingRight = 8;
                mainBadge.style.borderTopLeftRadius = 8;
                mainBadge.style.borderTopRightRadius = 8;
                mainBadge.style.borderBottomLeftRadius = 8;
                mainBadge.style.borderBottomRightRadius = 8;
                mainBadge.style.fontSize = 10;
                mainBadge.style.unityFontStyleAndWeight = FontStyle.Bold;
                mainBadge.style.marginLeft = 8;
                titleRow.Add(mainBadge);
            }

            infoContainer.Add(titleRow);

            var tagLabel = new Label($"Tag: {offering.Tag}");
            tagLabel.style.fontSize = 12;
            tagLabel.style.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            tagLabel.style.marginTop = 2;
            infoContainer.Add(tagLabel);

            leftContainer.Add(infoContainer);
            headerRow.Add(leftContainer);

            card.Add(headerRow);

            // Products section
            if (offering.Products != null && offering.Products.Count > 0)
            {
                var divider = new VisualElement();
                divider.style.height = 1;
                divider.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                divider.style.marginTop = 8;
                divider.style.marginBottom = 12;
                card.Add(divider);

                var productsHeader = new Label($"Products ({offering.Products.Count})");
                productsHeader.style.fontSize = 13;
                productsHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
                productsHeader.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                productsHeader.style.marginBottom = 12;
                card.Add(productsHeader);

                foreach (var product in offering.Products)
                {
                    var productRow = CreateProductRow(product);
                    card.Add(productRow);
                }
            }

            return card;
        }

        private VisualElement CreateProductRow(Product product)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            row.style.borderTopLeftRadius = 8;
            row.style.borderTopRightRadius = 8;
            row.style.borderBottomLeftRadius = 8;
            row.style.borderBottomRightRadius = 8;
            row.style.paddingTop = 10;
            row.style.paddingBottom = 10;
            row.style.paddingLeft = 12;
            row.style.paddingRight = 12;
            row.style.marginBottom = 8;

            // Product icon
            var icon = new Label("📦");
            icon.style.fontSize = 16;
            icon.style.marginRight = 10;
            row.Add(icon);

            // Product info
            var infoContainer = new VisualElement();
            infoContainer.style.flexGrow = 1;

            var titleLabel = new Label(product.StoreTitle ?? product.QonversionId);
            titleLabel.style.fontSize = 14;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.color = Color.white;
            infoContainer.Add(titleLabel);

            var storeIdLabel = new Label(product.StoreId ?? "No store ID");
            storeIdLabel.style.fontSize = 11;
            storeIdLabel.style.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            storeIdLabel.style.marginTop = 2;
            infoContainer.Add(storeIdLabel);

            row.Add(infoContainer);

            // Price
            var priceLabel = new Label(product.PrettyPrice ?? "N/A");
            priceLabel.style.fontSize = 14;
            priceLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            priceLabel.style.color = new Color(0.2f, 0.5f, 0.9f, 1f);
            row.Add(priceLabel);

            return row;
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
    }
}
