using EFCore_3;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace EFCore_3
{
    public class ShopService
    {
        // ==========================================
        // CRUD для OrdersSet (Замовлення)
        // ==========================================

        // CREATE
        public int CreateOrder(string quantity)
        {
            using (var db = new ShopContainer())
            {
                var order = new Orders { Quantity = quantity };
                db.OrdersSet.Add(order);
                db.SaveChanges();
                return order.Id; // Повертає згенерований БД ID
            }
        }

        // READ (Усі замовлення разом з їхніми продуктами)
        public List<Orders> GetAllOrders()
        {
            using (var db = new ShopContainer())
            {
                return db.OrdersSet.Include(o => o.Products).ToList();
            }
        }

        // READ (Одно замовлення за ID)
        public Orders GetOrderById(int id)
        {
            using (var db = new ShopContainer())
            {
                return db.OrdersSet.Include(o => o.Products).FirstOrDefault(o => o.Id == id);
            }
        }

        // UPDATE
        public void UpdateOrder(int id, string newQuantity)
        {
            using (var db = new ShopContainer())
            {
                var order = db.OrdersSet.Find(id);
                if (order != null)
                {
                    order.Quantity = newQuantity;
                    db.SaveChanges();
                }
            }
        }

        // DELETE
        public void DeleteOrder(int id)
        {
            using (var db = new ShopContainer())
            {
                var order = db.OrdersSet.Include(o => o.Products).FirstOrDefault(o => o.Id == id);
                if (order != null)
                {
                    // Спочатку видаляємо зв'язані продукти, щоб не порушити Foreign Key constraint
                    db.ProductsSet.RemoveRange(order.Products);
                    db.OrdersSet.Remove(order);
                    db.SaveChanges();
                }
            }
        }

        // ==========================================
        // CRUD для ProductsSet (Продукти)
        // ==========================================

        // CREATE
        public int CreateProduct(string name, int quantity, decimal price, int orderId)
        {
            using (var db = new ShopContainer())
            {
                // Перевірка чи існує таке замовлення
                if (!db.OrdersSet.Any(o => o.Id == orderId))
                    throw new Exception($"Замовлення з ID {orderId} не знайдено.");

                var product = new Products
                {
                    Name = name,
                    Quantity = quantity,
                    Price = price,
                    OrdersId = orderId
                };

                db.ProductsSet.Add(product);
                db.SaveChanges();
                return product.Id;
            }
        }

        // READ (Усі продукти)
        public List<Products> GetAllProducts()
        {
            using (var db = new ShopContainer())
            {
                return db.ProductsSet.ToList();
            }
        }

        // UPDATE
        public void UpdateProduct(int id, string name, int quantity, decimal price, int orderId)
        {
            using (var db = new ShopContainer())
            {
                var product = db.ProductsSet.Find(id);
                if (product != null)
                {
                    product.Name = name;
                    product.Quantity = quantity;
                    product.Price = price;
                    product.OrdersId = orderId;
                    db.SaveChanges();
                }
            }
        }

        // DELETE
        public void DeleteProduct(int id)
        {
            using (var db = new ShopContainer())
            {
                var product = db.ProductsSet.Find(id);
                if (product != null)
                {
                    db.ProductsSet.Remove(product);
                    db.SaveChanges();
                }
            }
        }
    }
}
