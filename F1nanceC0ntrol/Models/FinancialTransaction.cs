using System.ComponentModel.DataAnnotations;

namespace F1nanceC0ntrol.Models
{
    public class FinancialTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [DataType(DataType.Currency)]
        [Display(Name = "Valor")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        [DataType(DataType.Date)]
        [Display(Name = "Data da Transação")]
        public DateTime TransactionDate { get; set; }

        [Required(ErrorMessage = "O tipo de transação é obrigatório")]
        [Display(Name = "Tipo")]
        public TransactionType Type { get; set; }

        [Display(Name = "Categoria")]
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [Display(Name = "Observações")]
        [StringLength(500)]
        public string Notes { get; set; }
    }

    public enum TransactionType
    {
        [Display(Name = "Receita")]
        Income,
        [Display(Name = "Despesa")]
        Expense
    }

    public class Category
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }
    }

}
