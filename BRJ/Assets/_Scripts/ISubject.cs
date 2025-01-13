using System.Collections;

public interface ISubject
{
    public ArrayList Observers { get; set; }

    public void Attach(IObserver observer);

    public void Detach(IObserver observer);

    void NotifyObservers();
}
