using System;

namespace LegacySql.Domain.SellingPrices
{
    public class SellingPriceColumn
    {
        public string Title { get; }
        
        public SellingPriceColumn(int columnId)
        {
            Title = GetPriceColumnTitle(columnId);
        }
        
        private string GetPriceColumnTitle(int columnId)
        {
            switch (columnId)
            {
                case 1: return "Цена1";
                case 6: return "Цена0";
                case 2: return "Цена2";
                case 3: return "Цена3";
                case 9: return "Цена4";
                case 5: return "Цена5";
                case 11: return "ЦенаИ";
                case 8: return "SS";
            }

            throw new ArgumentException($"Не существует колонки цен с Id: {columnId}");
        }
    }
}