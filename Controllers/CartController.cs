using Microsoft.AspNetCore.Mvc;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories;
using System.Security.Claims;
using System.Linq.Expressions;

namespace movie_hospital_1.Controllers
{
    public class CartController : Controller
    {
        private readonly OrderRepository _orderRepository;

        public CartController(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

      
    }
}