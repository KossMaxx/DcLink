using System;

namespace LegacySql.Legacy.Data.Products
{
    internal class PictureData
    {
        public int Product_pic_row_id { get; set; }
        public int Product_pic_Id { get; set; }
        public int Product_pic_ProductId { get; set; }
        public string Product_pic_Url { get; set; }
        public DateTime Product_pic_Date { get; set; }
        public string Product_pic_Uuu { get; set; }
        public byte Product_pic_CobraPic { get; set; }
    }
}
