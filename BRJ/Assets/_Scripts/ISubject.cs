using System.Collections;

/* FILE HEADER
 * AUTHOR: Chase Morgan | CREATED: 01/12/2025
 * UPDATED: 01/12/2025 | BY: Chase Morgan | COMMENTS: Added interface
 * FILE DESCRIPTION: ISubject interface
 */

public interface ISubject
{
    public ArrayList Observers { get; set; }

    public void Attach(IObserver observer);

    public void Detach(IObserver observer);

    void NotifyObservers();
}
