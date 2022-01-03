using Ivao.It.DiscordLogin.Models;

namespace Ivao.It.DiscordLogin.ApiDto;

public class DiscordWebHookContent
{
    public string? Username { get; init; }
    public List<DiscordEmbed> Embeds { get; init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private DiscordWebHookContent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    public static DiscordWebHookContent BuildTemplated(IvaoScheduledEvent evt)
    {
        return new DiscordWebHookContent
        {
            Username = "IVAO IT Training Department", //Se altro tipo di event, può cambiare l'autore,
            Embeds = new List<DiscordEmbed> { DiscordEmbed.BuildTemplated(evt) }
        };
    }
    public static DiscordWebHookContent BuildTemplated(IEnumerable<IvaoScheduledEvent> events)
    {
        var root = new DiscordWebHookContent
        {
            Username = "IVAO IT Training Department", //Se altro tipo di event, può cambiare l'autore
            Embeds = new List<DiscordEmbed>()
        };

        foreach (var evt in events)
            root.AddEmbed(evt);

        return root;
    }


    public void AddEmbed(IvaoScheduledEvent evt)
        => Embeds.Add(DiscordEmbed.BuildTemplated(evt));


    public class DiscordEmbed
    {
        public string Title { get; init; }
        public EmbedColor Color { get; init; }
        public string Description { get; init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private DiscordEmbed()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        internal static DiscordEmbed BuildTemplated(IvaoScheduledEvent evt)
        {
            var stringTemplate = TemplateToString(evt.Type);
            return new DiscordEmbed
            {
                Title = $"{stringTemplate}!\n{RatingToStars(evt.Rating)}",
                Color = evt.Type == EventType.Exam ? EmbedColor.Red : EmbedColor.Blue,
                Description = $"C'è un **{stringTemplate}** programmato per oggi:\n\n{RatingToIcon(evt.Rating)} **{evt.Facility}** ore __{evt.Planned:HHmm}Z__ \n\n" +
                    $"**Go around, round robin ed emergenze** da coordinare con l'esaminatore.\n\n" +
                    $"Ricordiamo la possibilità di ricevere il [Pilot Support Award](https://tours.ivao.it/welcome/rules) il quale verrà assegnato ai piloti che partecipando ai training/esami ne faranno richiesta."
            };
        }

        private static string TemplateToString(EventType template) => template switch
        {
            EventType.Exam => "Esame",
            EventType.Training => "Training",
            _ => throw new ArgumentOutOfRangeException(nameof(template), $"Not expected direction value: {template}"),
        };

        private static string RatingToIcon(AtcRating rating) => rating switch
        {
            AtcRating.ADC => ":airplane_departure:",
            AtcRating.APC => ":satellite:",
            AtcRating.ACC => ":tokyo_tower:",
            _ => throw new ArgumentOutOfRangeException(nameof(rating), $"Not expected direction value: {rating}"),
        };

        private static string RatingToStars(AtcRating rating)
             => string.Join("", Enumerable.Range(0, (int)rating - 4).Select(r => ":star:"));

        //private static string RatingToStars(AtcRating rating) => rating switch
        //{
        //    AtcRating.ADC => ":star:",
        //    AtcRating.APC => ":star::star:",
        //    AtcRating.ACC => ":star::star::star:",
        //    _ => throw new ArgumentOutOfRangeException(nameof(rating), $"Not expected direction value: {rating}"),
        //};

        public enum EmbedColor
        {
            Blue = 863385,
            Red = 13369344,
        }
    }
}
