using System;
using PropertyChanged;

[ImplementPropertyChanged]	
public class TipCalcModel 
{
	public string SubTotal { get; set; }
	public string PostTaxTotal { get; set; }
	public string TipPercent { get; set; }
	public decimal? TipAmount { get; set; }
	public decimal? Total { get; set; }
	
//	public decimal? TipAmount { 
//		get {
//			decimal tipPercent, subTotal;
//			if (Decimal.TryParse (this.TipPercent, out tipPercent) && Decimal.TryParse (this.SubTotal, out subTotal)) {
//				return Math.Round (tipPercent * subTotal / 100, 2); 
//			} else
//				return null;
//		} 
//	}
//	public decimal? Total { 
//		get { 
//			decimal postTaxTotal;
//			if (Decimal.TryParse (this.PostTaxTotal, out postTaxTotal) && this.TipAmount.HasValue) {
//				return Math.Round(4 * (postTaxTotal + this.TipAmount.Value)) / 4; 
//			} else
//				return null;
//		} 
//	}
}
