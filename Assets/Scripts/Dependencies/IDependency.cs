namespace QuizCinema
{
    public interface IDependency<T>
    {
        void Construct(T obj);
    }
}