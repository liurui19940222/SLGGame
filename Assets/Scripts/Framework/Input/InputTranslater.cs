namespace Framework.Input
{

    public static class InputTranslater
    {
        public static bool IsAttackWord(EInputWord word)
        {
            return word == EInputWord.LB || word == EInputWord.RB || word == EInputWord.RT || word == EInputWord.LT;
        }
    }

}