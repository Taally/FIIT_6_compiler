namespace SimpleLang
{
    public static class ThreeAddressCodeTmp
    {
        private static int tmpInd = 1, tmpLabelInd = 1;
        public static string GenTmpName() => "#t" + tmpInd++;
        public static string GenTmpLabel() => "L" + tmpLabelInd++;
        public static void ResetTmpName() => tmpInd = 1;
        public static void ResetTmpLabel() => tmpLabelInd = 1;
    }
}
