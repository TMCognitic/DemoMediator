namespace DemoMediator
{
    class Program
    {
        static void Main(string[] args)
        {
            Personne mehdy = new Personne("Mehdy");
            Personne jeremy = new Personne("Jeremy");
            Personne paul = new Personne("Paul");

            mehdy.SendMessage("Bonjour!");
            paul.SendMessage("Salut!");

            mehdy.SendMessage(paul, "Salut mon lapin!");
        }
    }

    class Personne
    {
        public string Name { get; init; } 

        public Personne(string name)
        {
            Name = name;
            Mediator<string>.Default.Register(LireMessage);
            Mediator<string>.Default.Register(this, LireMessagePrive);
        }

        ~Personne()
        {
            Mediator<string>.Default.Unregister(LireMessage);
            Mediator<string>.Default.Unregister(this, LireMessagePrive);
        }

        public void LireMessage(object sender, string message)
        {
            if(sender != this)
                Console.WriteLine($"{Name} lit le message : {message}");
        }

        public void LireMessagePrive(object sender, string message)
        {
            if (sender != this)
                Console.WriteLine($"{Name} lit le message privé : {message}");
        }

        public void SendMessage(string message)
        {
            Mediator<string>.Default.Send(this, message);
        }

        public void SendMessage(object receiver, string message)
        {
            Mediator<string>.Default.Send(this, receiver, message);
        }
    }

    public class Mediator<TMessage>
    {
        private static Mediator<TMessage>? _default;

        public static Mediator<TMessage> Default
        {
            get
            {
                return _default ??= new Mediator<TMessage>();
            }
        }

        private Action<object, TMessage>? _broadcast;
        Dictionary<object, Action<object, TMessage>?> _boxes;

        public Mediator()
        {
            _boxes = new Dictionary<object, Action<object, TMessage>?>();
        }

        public void Register(object receiver, Action<object, TMessage> action)
        {
            if(!_boxes.ContainsKey(receiver))
                _boxes.Add(receiver, null);

            _boxes[receiver] += action;
        }

        public void Unregister(Action<object, TMessage> action) 
        {
            _broadcast -= action;
        }

        public void Unregister(object receiver, Action<object, TMessage> action)
        {
            _boxes.Remove(receiver);
        }

        public void Register(Action<object, TMessage> action)
        {
            _broadcast += action;
        }

        public void Send(object sender, TMessage message)
        {
            _broadcast?.Invoke(sender, message);
        }

        public void Send(object sender, object receiver, TMessage message)
        {
            if(!_boxes.ContainsKey(receiver)) 
            {
                throw new InvalidOperationException();
            }

            _boxes[receiver]?.Invoke(sender, message);
        }
    }
}
