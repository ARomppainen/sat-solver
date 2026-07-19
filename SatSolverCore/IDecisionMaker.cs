namespace SatSolverCore;

public interface IDecisionMaker
{
    int ChooseUnassignedLiteral(IPartialAssignment assignment);
}
