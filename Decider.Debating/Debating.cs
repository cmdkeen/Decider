using System;
using System.Collections.Generic;
using System.Linq;
using Decider.Csp.BaseTypes;
using Decider.Csp.Integer;
using Decider.Csp.Global;

namespace Decider.Debating
{
    public static class Debating
    {
        public class Team
        {
            public int Id { get; set; }
            public int Score { get; set; }
            public int FirstProp { get; set; }
            public int SecondProp { get; set; }
            public int FirstOpp { get; set; }
            public int SecondOpp { get; set; }

            public Team(int id, int score, int fp, int sp, int fo, int so)
            {
                Id = id;
                Score = score;
                FirstProp = fp;
                SecondProp = sp;
                FirstOpp = fo;
                SecondOpp = so;
            }
        }

        static void Main(string[] args)
        {
            var badnessFactor = 3;
            var teams = new[]
            {
                new Team(0, 3, 1, 0, 0, 0),
                new Team(1, 2, 0, 1, 0, 0),
                new Team(2, 1, 0, 0, 1, 0),
                new Team(3, 0, 0, 0, 0, 1),
                new Team(4, 3, 0, 0, 0, 1),
                new Team(5, 2, 0, 0, 1, 0),
                new Team(6, 1, 0, 1, 0, 0),
                new Team(7, 0, 1, 0, 0, 0)
            };
            Array.Sort(teams, (x, y) => y.Score.CompareTo(x.Score)); //High to low
            var rooms = teams.Length / 4;

            //Positions[room * position] = TeamId
            var positions = new List<VariableInteger>(teams.Length);
            var maxScore = teams.Max(x => x.Score);
            var roomsForMax = (int)Math.Ceiling(teams.Count(x => x.Score == maxScore) / 4.0);
            for (var i = 0; i < teams.Length; i++)
            {
                //if (teams[i].Score == maxScore)
                //{
                //    positions.Add(new VariableInteger($"Team_{i}", 0, roomsForMax));
                //}
                //else {  }
                positions.Add(new VariableInteger($"Team_{i}", 0, teams.Length - 1));
            }



            //Scores[teamId] = current score
            var scores = new ConstrainedArray(teams.Select(x => x.Score));

            //
            var firstProp = new ConstrainedArray(teams.Select(x => x.FirstProp * badnessFactor));
            var secondProp = new ConstrainedArray(teams.Select(x => x.SecondProp * badnessFactor));
            var firstOpp = new ConstrainedArray(teams.Select(x => x.FirstOpp * badnessFactor));
            var secondOpp = new ConstrainedArray(teams.Select(x => x.SecondOpp * badnessFactor));

            var constraints = new List<IConstraint>();
            constraints.Add(new AllDifferentInteger(positions));

            for (var i = 0; i < rooms - 1; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var x = positions[i * 4 + j];

                    for (int k = 0; k < 4; k++)
                    {
                        var y = positions[(i + 1) * 4 + k];
                        constraints.Add(new ConstraintInteger(scores[x] >= scores[y]));
                    }
                }
            }

            IState<int> state = new StateInteger(positions, constraints);
            StateOperationResult searchResult;
            state.StartSearch(out searchResult);

            Console.WriteLine("Runtime:\t{0}\nBacktracks:\t{1}\n", state.Runtime, state.Backtracks);

            Console.WriteLine("Room | 1P | 1O | 2P | 2O | Scores ");
            for (var i = 0; i < rooms; i++)
            {
                var fp = positions[i * 4].Value;
                var sp = positions[i * 4 + 1].Value;
                var fo = positions[i * 4 + 2].Value;
                var so = positions[i * 4 + 3].Value;

                Console.WriteLine("{0,-5}|{1,3} |{2,3} |{3,3} |{4,3} | {5} {6} {7} {8}", i, fp, sp, fo, so, scores[fp], scores[sp], scores[fo], scores[so]);
            }

            Console.ReadKey();
        }
    }
}
