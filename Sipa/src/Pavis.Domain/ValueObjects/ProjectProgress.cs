namespace Pavis.Domain.ValueObjects;

public class ProjectProgress
{
    public int Technical { get; private set; }
    public int Legal { get; private set; }
    public int Financial { get; private set; }
    public int Social { get; private set; }

    public ProjectProgress(int technical = 0, int legal = 0, int financial = 0, int social = 0)
    {
        ValidatePercentage(technical);
        ValidatePercentage(legal);
        ValidatePercentage(financial);
        ValidatePercentage(social);

        Technical = technical;
        Legal = legal;
        Financial = financial;
        Social = social;
    }

    private void ValidatePercentage(int value)
    {
        if (value < 0 || value > 100)
        {
            throw new ArgumentException("Percentage must be between 0 and 100");
        }
    }

    public void Update(int technical, int legal, int financial, int social)
    {
        Technical = technical;
        Legal = legal;
        Financial = financial;
        Social = social;
    }
}
