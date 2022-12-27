namespace Mdfry1.Entities.Values;

public static class LampDialogInteractions
{
    public const string PromptForUsingFluid = "/Interactions/LampFluidInteractions/PromptForUsingFluid";
    public const string PromptForNeedFluid = "/Interactions/LampFluidInteractions/PromptForNeedFluid";
    public const string PromptForFluidFull = "/Interactions/LampFluidInteractions/PromptForFluidFull";
    public const string LampFluidFound = "/Interactions/LampFluidInteractions/LampFluidFound";

    public static readonly string[] Dialogs =
    {
        PromptForUsingFluid,
        PromptForNeedFluid,
        PromptForFluidFull,
        LampFluidFound
    };
}