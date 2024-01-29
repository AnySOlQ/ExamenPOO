using System;
using System.Collections.Generic;
using System.Linq;

namespace Examen_Parcial
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PRODUCTS");
            Console.WriteLine("-------------------------------------------------");

            Product product1 = new FixedPriceProduct()
            {
                Description = "Vino Gato Negro",
                Id = 1010,
                Price = 46000M,
                Tax = 0.19F
            };

            Product product3 = new VariablePriceProduct()
            {
                Description = "Queso Holandes",
                Id = 3030,
                Measurement = "Kilo",
                Price = 32000M,
                Quantity = 0.536F,
                Tax = 0.19F
            };

            Product product5 = new ComposedProduct()
            {
                Description = "Ancheta #1",
                Discount = 0.12F,
                Id = 5050,
                Products = new List<Product>() { product1, product3 }
            };

            Invoice invoice = new Invoice();
            invoice.AddProduct(product1);
            invoice.AddProduct(product3);
            invoice.AddProduct(product5);

            Console.WriteLine("RECEIPT");
            Console.WriteLine("-------------------------------------------------");

            foreach (var line in invoice.GenerateReceipt().Split('\n'))
            {
                Console.WriteLine(line);
            }

            Console.WriteLine("                   =============");
            Console.WriteLine($"TOTAL:               {invoice.CalculateTotal():C}");
        }
    }

    internal class Product
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public float Tax { get; set; }

        public virtual string Print()
        {
            return $"{Id} {Description}\n" +
                   $"      Price......:   {Price:C}\n" +
                   $"      Tax........:        {Tax:P}\n" +
                   $"      Value......:   {CalculateValue():C}\n";
        }

        public virtual decimal CalculateValue()
        {
            return Price * (1 + (decimal)Tax);
        }
    }

    internal class FixedPriceProduct : Product
    {
    }

    internal class VariablePriceProduct : Product
    {
        public string Measurement { get; set; }
        public float? Quantity { get; set; }

        public override string Print()
        {
            return $"{base.Print()}" +
                   $"      Measurement: {Measurement}\n" +
                   $"      Quantity...:         {Quantity}\n";
        }

        public override decimal CalculateValue()
        {
            float quantityValue = Quantity ?? 0; 
            decimal decimalQuantityValue = Convert.ToDecimal(quantityValue);
            return Price * decimalQuantityValue * (1 + (decimal)Tax);
        }
    }

    internal class ComposedProduct : Product
    {
        public float Discount { get; set; }
        public List<Product> Products { get; set; }

        public override string Print()
        {
            string output = $"{base.Print()}" +
                            $"      Discount...:        {Discount:P}\n";

            foreach (var product in Products)
            {
                output += $"{product.Print()}";
            }

            output += $"      Value......:   {CalculateValue():C}\n";

            return output;
        }

        public override decimal CalculateValue()
        {
            decimal total = Products.Sum(product => product.CalculateValue());
            return total * (1 - (decimal)Discount);
        }
    }

    internal class Invoice
    {
        private List<Product> products;

        public Invoice()
        {
            products = new List<Product>();
        }

        public void AddProduct(Product product)
        {
            products.Add(product);
        }

        public string GenerateReceipt()
        {
            string receipt = "";

            foreach (var product in products)
            {
                receipt += $"{product.Print()}\n";
            }

            return receipt;
        }

        public decimal CalculateTotal()
        {
            decimal totalAmount = products.Sum(product => product.CalculateValue());
            return totalAmount;
        }
    }
}
