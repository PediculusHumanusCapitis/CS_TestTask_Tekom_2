namespace WhatAreWeEating.Models
{
    public class UserDish
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DishId { get; set; }
        public DateTime Date { get; set; }

        public virtual User User { get; set; }
        public virtual Dish Dish { get; set; }
    }
}
