using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Services.Abstraction


{
    public interface IOrderService
    {
        List<Order> GetOrdersByUserId(int id);
        Order GetOrderById(int id);
        void UpdateOrder(Order order);
        List<Order> GetAllOrders();
        void AddOrder(Order order);
        void DeleteOrder(int id);
    }
}
