public class DigitalGoodModel : BaseModel {
    public int shards;
    public int sprName;
	public bool isSale;
    public CATEGORY_TYPE category;
    public ITEM_TYPE itemType;
}

public enum CATEGORY_TYPE {
	ALL = 0,
    CONSUMABLE,
    UPGRADES
}

public enum ITEM_TYPE {
	ALL = 0,
    HEALTH,
    MANA,
    SORCERY,
    WEAPONS,
    ARMOR,
    ACCESSORIES
}
