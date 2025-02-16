﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Domain.FreshMenuVotes;

public static class FreshMenuVoteErrors
{
    public static readonly Error VotePlacedError = new("FreshMenuVote.VotePlacedError", "Error placing fresh menu vote.", ErrorType.Problem);
    public static readonly Error VoteNotAValidFreshMenuMealItemError = new("FreshMenuVote.VoteNotAValidFreshMenuMealItemError", "FreshMenu Meal is not valid.", ErrorType.Validation);
}
