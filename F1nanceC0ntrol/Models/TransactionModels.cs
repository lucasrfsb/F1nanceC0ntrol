using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1nanceC0ntrol.Models
{
    // Classe base para todos os tipos de transação
    public abstract class BaseTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [DataType(DataType.Currency)]
        [Display(Name = "Valor")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        [Display(Name = "Categoria")]
        public int? CategoryId { get; set; }

        [Display(Name = "Categoria")]
        public virtual Category Category { get; set; }
    }

    // 1. Comissionamento dos vendedores
    public class SellerCommission : BaseTransaction
    {
        [Required(ErrorMessage = "O nome do funcionário é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome do funcionário deve ter no máximo 100 caracteres")]
        [Display(Name = "Nome do Funcionário")]
        public string EmployeeName { get; set; }
    }

    // 2. Custo diário operação
    public class DailyOperationCost : BaseTransaction
    {
        // Utiliza apenas os campos da classe base
    }

    // 3. Custos fixos
    public class FixedCost : BaseTransaction
    {
        // Utiliza apenas os campos da classe base
    }

    // 4. Custo de pós venda
    public class AfterSaleCost : BaseTransaction
    {
        [Required(ErrorMessage = "A placa é obrigatória")]
        [StringLength(10, ErrorMessage = "A placa deve ter no máximo 10 caracteres")]
        [Display(Name = "Placa")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "O carro é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome do carro deve ter no máximo 100 caracteres")]
        [Display(Name = "Carro")]
        public string Car { get; set; }
    }

    // 5. Custo dos carros
    public class CarCost : BaseTransaction
    {
        [Required(ErrorMessage = "A placa é obrigatória")]
        [StringLength(10, ErrorMessage = "A placa deve ter no máximo 10 caracteres")]
        [Display(Name = "Placa")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "O carro é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome do carro deve ter no máximo 100 caracteres")]
        [Display(Name = "Carro")]
        public string Car { get; set; }
    }

    // 6. Lucro retorno de financiamento
    public class FinancingReturn : BaseTransaction
    {
        [Required(ErrorMessage = "O banco é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome do banco deve ter no máximo 100 caracteres")]
        [Display(Name = "Banco")]
        public string Bank { get; set; }
    }

    // Classe para categorias
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
        [Display(Name = "Descrição")]
        public string Description { get; set; }
    }

    // Enum para tipos de transação (mantido para referência ou uso futuro)
    public enum TransactionType
    {
        [Display(Name = "Receita")]
        Income,
        [Display(Name = "Despesa")]
        Expense
    }
}