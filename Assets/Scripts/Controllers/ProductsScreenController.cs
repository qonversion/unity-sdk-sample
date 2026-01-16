using UnityEngine;
using UnityEngine.UIElements;
using QonversionUnity;

namespace QonversionSample
{
    /// <summary>
    /// Controller for the Products screen.
    /// Displays products and allows purchasing.
    /// </summary>
    public class ProductsScreenController : BaseScreenController
    {
        private VisualElement _productsContainer;
        private Label _emptyLabel;

        public ProductsScreenController(AppState appState) : base(appState) { }

        public override VisualElement CreateUI()
        {
            RootElement = new VisualElement();
            RootElement.style.flexGrow = 1;

            // Header with reload button
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.justifyContent = Justify.SpaceBetween;
            header.style.alignItems = Align.Center;
            header.style.marginBottom = 6;

            var title = new Label("Products");
            title.style.fontSize = 11;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = Color.white;
            header.Add(title);

            var reloadButton = CreateButton("Reload", LoadProducts);
            header.Add(reloadButton);

            RootElement.Add(header);

            // Empty state label
            _emptyLabel = new Label("No products. Click 'Reload'.");
            _emptyLabel.style.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            _emptyLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
            _emptyLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _emptyLabel.style.marginTop = 16;
            _emptyLabel.style.fontSize = 8;
            RootElement.Add(_emptyLabel);

            // Products container (scrollable)
            var scrollView = CreateScrollView();
            _productsContainer = new VisualElement();
            scrollView.Add(_productsContainer);
            RootElement.Add(scrollView);

            Refresh();
            return RootElement;
        }

        public override void Refresh()
        {
            if (_productsContainer == null) return;
            _productsContainer.Clear();

            var products = AppState.Products;
            
            if (products == null || products.Count == 0)
            {
                _emptyLabel.style.display = DisplayStyle.Flex;
                return;
            }

            _emptyLabel.style.display = DisplayStyle.None;

            foreach (var kvp in products)
            {
                var product = kvp.Value;
                var card = CreateProductCard(product);
                _productsContainer.Add(card);
            }
        }

        private VisualElement CreateProductCard(Product product)
        {
            var card = CreateCard();

            // Header row
            var headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            headerRow.style.justifyContent = Justify.SpaceBetween;
            headerRow.style.alignItems = Align.FlexStart;
            headerRow.style.marginBottom = 4;
            headerRow.style.overflow = Overflow.Hidden;

            // Product info
            var infoContainer = new VisualElement();
            infoContainer.style.flexGrow = 1;
            infoContainer.style.flexShrink = 1;
            infoContainer.style.overflow = Overflow.Hidden;
            infoContainer.style.marginRight = 6;

            var titleLabel = new Label(product.StoreTitle ?? product.QonversionId);
            titleLabel.style.fontSize = 9;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.color = Color.white;
            titleLabel.style.whiteSpace = WhiteSpace.Normal;
            infoContainer.Add(titleLabel);

            var storeIdLabel = new Label(product.StoreId ?? "No store ID");
            storeIdLabel.style.fontSize = 7;
            storeIdLabel.style.color = new Color(0.5f, 0.5f, 0.55f, 1f);
            storeIdLabel.style.marginTop = 1;
            storeIdLabel.style.whiteSpace = WhiteSpace.Normal;
            infoContainer.Add(storeIdLabel);

            headerRow.Add(infoContainer);

            // Price badge
            var priceLabel = CreateChip(product.PrettyPrice ?? "N/A", new Color(0.2f, 0.5f, 0.9f, 1f));
            headerRow.Add(priceLabel);

            card.Add(headerRow);

            // Description
            var description = product.StoreDescription;
            if (!string.IsNullOrEmpty(description))
            {
                var descLabel = new Label(description);
                descLabel.style.fontSize = 7;
                descLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                descLabel.style.marginBottom = 4;
                descLabel.style.whiteSpace = WhiteSpace.Normal;
                card.Add(descLabel);
            }

            // Chips row
            var chipsRow = new VisualElement();
            chipsRow.style.flexDirection = FlexDirection.Row;
            chipsRow.style.flexWrap = Wrap.Wrap;
            chipsRow.style.marginBottom = 4;
            chipsRow.style.overflow = Overflow.Hidden;

            chipsRow.Add(CreateChip($"{product.Type}", new Color(0.3f, 0.3f, 0.35f, 1f)));

            if (product.SubscriptionPeriod != null)
            {
                var period = product.SubscriptionPeriod;
                chipsRow.Add(CreateChip($"{period.UnitCount} {period.Unit}", new Color(0.3f, 0.3f, 0.35f, 1f)));
            }

            card.Add(chipsRow);

            // Purchase button
            var purchaseButton = CreateButton("Purchase", () => PurchaseProduct(product));
            purchaseButton.style.width = new StyleLength(StyleKeyword.Auto);
            card.Add(purchaseButton);

            return card;
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

        private void PurchaseProduct(Product product)
        {
            Debug.Log($"🔄 [Qonversion] Purchasing product: {product.QonversionId}...");
            AppState.SetLoading(true);

            Qonversion.GetSharedInstance().Purchase(product, (result) =>
            {
                AppState.SetLoading(false);
                
                // Log full PurchaseResult details
                LogPurchaseResult(result);

                if (result.IsCanceled)
                {
                    Debug.Log("ℹ️ [Qonversion] Purchase cancelled by user");
                    AppState.ShowSuccess("Purchase cancelled");
                    return;
                }

                if (result.Error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Purchase failed: {result.Error.Message}");
                    AppState.ShowError($"Purchase failed: {result.Error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] Purchase successful: {result.Entitlements?.Count ?? 0} entitlements");
                if (result.Entitlements != null)
                {
                    AppState.SetEntitlements(result.Entitlements);
                }
                AppState.ShowSuccess($"Purchase successful! Status: {result.Status}, Entitlements: {result.Entitlements?.Count ?? 0}");
            });
        }
        
        private void LogPurchaseResult(PurchaseResult result)
        {
            Debug.Log("=== Purchase Result ===");
            Debug.Log($"  Status: {result.Status}");
            Debug.Log($"  Source: {result.Source}");
            Debug.Log($"  IsFallbackGenerated: {result.IsFallbackGenerated}");
            Debug.Log($"  IsSuccess: {result.IsSuccess}");
            Debug.Log($"  IsCanceled: {result.IsCanceled}");
            Debug.Log($"  IsPending: {result.IsPending}");
            Debug.Log($"  IsError: {result.IsError}");
            
            if (result.Error != null)
            {
                Debug.Log($"  Error Code: {result.Error.Code}");
                Debug.Log($"  Error Message: {result.Error.Message}");
            }
            
            if (result.StoreTransaction != null)
            {
                var tx = result.StoreTransaction;
                Debug.Log("  --- Store Transaction ---");
                Debug.Log($"    Transaction ID: {tx.TransactionId ?? "N/A"}");
                Debug.Log($"    Original TX ID: {tx.OriginalTransactionId ?? "N/A"}");
                Debug.Log($"    Product ID: {tx.ProductId ?? "N/A"}");
                Debug.Log($"    Quantity: {tx.Quantity}");
                Debug.Log($"    Transaction Date: {tx.TransactionDate?.ToString() ?? "N/A"}");
                Debug.Log($"    Promo Offer ID: {tx.PromoOfferId ?? "N/A"}");
                Debug.Log($"    Purchase Token: {(tx.PurchaseToken != null ? tx.PurchaseToken.Substring(0, System.Math.Min(20, tx.PurchaseToken.Length)) + "..." : "N/A")}");
            }
            
            if (result.Entitlements != null && result.Entitlements.Count > 0)
            {
                Debug.Log($"  --- Entitlements ({result.Entitlements.Count}) ---");
                foreach (var pair in result.Entitlements)
                {
                    Debug.Log($"    • {pair.Key} (active: {pair.Value.IsActive})");
                }
            }
            
            Debug.Log("=======================");
        }
    }
}
