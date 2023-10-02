using System.Collections.Generic;

public class RoadEventData
{
    public List<Choice> Choices { get; } = new List<Choice>();
    public string Title { get; set; }
    public string Description { get; set; }
}

public class Choice {
    public string ChoiceDescription { get; set; }
    public List<ChoiceEffect> ChoiceEffects { get; set; } = new List<ChoiceEffect>();
}

public class ChoiceEffect {
	public int ChoiceValue { get; set; }
    public ChoiceType ChoiceType { get; set; }
	public ResourceType ResourceType { get; set; }
	public ObjectVisualType ObjectVisualType { get; set; }
	public int Insight { get; set; }
	public int InsightRequired { get; set; }
}

public enum ChoiceType {
    Nothing,
    Object,
    Resource,
    Brawl,
    Insight
}

public enum BrawlDifficulty {
    None,
    Easy,
    Medium,
    Hard
}