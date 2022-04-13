using System;

namespace WebApplication2.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime Created { get; set; }
        public string Text { get; set; }
        public string RecieverId { get; set; }
        public string SenderId { get; set; }
    }
}