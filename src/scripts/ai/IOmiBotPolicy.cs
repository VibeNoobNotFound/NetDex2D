using System.Threading;
using NetDex.Core.Commands;
using NetDex.Core.Rules;

namespace NetDex.AI;

public interface IOmiBotPolicy
{
    MatchCommand ChooseCommand(BotPerceptionState perception, IGameRulesEngine rulesEngine, CancellationToken cancellationToken);
}
