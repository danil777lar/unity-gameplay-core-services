namespace Larje.Core.Monetization
{
    public readonly struct ProductInfo
    {
        public readonly bool Available;
        public readonly string PriceString;
        public readonly string Title;
        public readonly string Description;
        public readonly string CurrencyCode;
        public readonly decimal Price;

        public ProductInfo(bool available, string priceString, string title, string description, string currencyCode, decimal price)
        {
            Available = available;
            PriceString = priceString;
            Title = title;
            Description = description;
            CurrencyCode = currencyCode;
            Price = price;
        }
    }
}
