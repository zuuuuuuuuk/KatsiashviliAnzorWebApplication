﻿namespace KatsiashviliAnzorWebApplication.Dto
{
    public class PromoCodeDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsGlobal { get; set; }
        public int? SourcePromoId { get; set; }
    }
}
