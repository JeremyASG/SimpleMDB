using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleMDB
{
    public class MockActorRepository : IActorRepository
    {
        private List<Actor> actors;
        private int idCount;

        public MockActorRepository()
        {
            actors = new List<Actor>();
            idCount = 0;

            var firstNames = new[]
            {

                "James", "Mary", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "William", "Elizabeth",
                "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Charles", "Karen",
                "Christopher", "Nancy", "Daniel", "Lisa", "Matthew", "Betty", "Anthony", "Margaret", "Mark", "Sandra",
                "Donald", "Ashley", "Steven", "Kimberly", "Paul", "Emily", "Andrew", "Donna", "Joshua", "Michelle",
                "Kenneth", "Dorothy", "Kevin", "Carol", "Brian", "Amanda", "George", "Melissa", "Edward", "Deborah",
                "Ronald", "Stephanie", "Timothy", "Rebecca", "Jason", "Sharon", "Jeffrey", "Laura", "Ryan", "Cynthia",
                "Jacob", "Kathleen", "Gary", "Amy", "Nicholas", "Shirley", "Eric", "Angela", "Stephen", "Helen",
                "Jonathan", "Anna", "Larry", "Brenda", "Justin", "Pamela", "Scott", "Nicole", "Brandon", "Emma",
                "Frank", "Samantha", "Benjamin", "Katherine", "Gregory", "Christine", "Samuel", "Debra", "Raymond", "Rachel",
                "Patrick", "Catherine", "Alexander", "Carolyn", "Jack", "Janet", "Dennis", "Ruth", "Jerry", "Maria"
            };

            var lastNames = new[]
            {

                "Johnson", "Williams", "Brown", "Jones", "Miller", "Davis", "Rodriguez", "Martinez",
                "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Taylor", "Moore", "Jackson", "Martin",
                "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
                "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
                "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts",
                "Gomez", "Phillips", "Evans", "Turner", "Diaz", "Parker", "Cruz", "Edwards", "Collins", "Reyes",
                "Stewart", "Morris", "Morales", "Murphy", "Cook", "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper",
                "Peterson", "Bailey", "Reed", "Kelly", "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson",
                "Watson", "Brooks", "Chavez", "Wood", "James", "Bennett", "Gray", "Mendoza", "Ruiz", "Hughes",
                "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers", "Long", "Ross", "Foster", "Jimenez"
            };

            var highlights = new[]
            {
                "drama and action films",
                "comedies and musicals",
                "independent art-house features",
                "television drama series",
                "blockbuster sci-fi movies",
                "thriller and horror genres",
                "historical biopics",
                "romantic comedies",
                "animated feature films",
                "stage and screen productions"
            };

            var random = new Random();

            for (int i = 0; i < 100; i++)
            {
                string firstName = firstNames[random.Next(firstNames.Length)];
                string lastName = lastNames[random.Next(lastNames.Length)];
                string career = highlights[i % highlights.Length];
                float rating = (float)Math.Round(random.NextDouble() * 10, 1);

                string bio = $"{firstName} {lastName} is an actor known for their work in {career}. " +
                             "Over their career, they have garnered critical acclaim and a devoted fan base.";

                actors.Add(new Actor(idCount++, firstName, lastName, bio, rating));
            }
        }

        public async Task<PagedResult<Actor>> ReadAll(int page, int size)
        {
            int totalCount = actors.Count;
            int start = Math.Clamp((page - 1) * size, 0, totalCount);
            int length = Math.Clamp(size, 0, totalCount - start);

            List<Actor> values = actors.GetRange(start, length);

            var pagedResult = new PagedResult<Actor>(values, totalCount);

            return await Task.FromResult(pagedResult);
        }

        public async Task<Actor?> Create(Actor newActor)
        {
            newActor.Id = idCount++;
            actors.Add(newActor);

            return await Task.FromResult(newActor);
        }

        public async Task<Actor?> Read(int id)
        {
            Actor? actor = actors.FirstOrDefault(u => u.Id == id);
            return await Task.FromResult(actor);
        }

        public async Task<Actor?> Update(int id, Actor newActor)
        {
            Actor? actor = actors.FirstOrDefault(u => u.Id == id);

            if (actor != null)
            {
                actor.FirstName = newActor.FirstName;
                actor.LastName = newActor.LastName;
                actor.Bio = newActor.Bio;
                actor.Rating = newActor.Rating;
            }

            return await Task.FromResult(actor);
        }

        public async Task<Actor?> Delete(int id)
        {
            Actor? actor = actors.FirstOrDefault(u => u.Id == id);

            if (actor != null)
            {
                actors.Remove(actor);
            }

            return await Task.FromResult(actor);
        }
    }
}


