namespace Project.Model.Networking
{
    public class Wrapper<T>
    {
        public bool WaitForResponse;

        //class accessor directly to Value

        public Wrapper(T init)
        {
            Value = init;
        }

        public T Value { get; set; }

        public static implicit operator T(Wrapper<T> m)
        {
            return m.Value;
        }
    }
}