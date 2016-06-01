namespace Biblioteca.Entidad
{
    public class Message
    {
        public bool Status { get; set; }
        public string Mensaje { get; set; }

        public Message(bool status, string message = null)
        {
            Status = status;
            Mensaje = message;
        }

        public override string ToString()
        {
            return Mensaje ?? string.Empty;
        }
    }
}