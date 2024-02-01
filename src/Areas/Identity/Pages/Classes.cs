namespace MinimalIdentityUI.Areas.Identity.Pages
{
    public class Classes
    {
        public const string LargePrimaryButtonFullWidth = LargePrimaryButton + "w-full";
        public const string LargePrimaryButton = "no-underline py-3 px-4 inline-flex justify-center items-center gap-x-2 text-sm font-semibold rounded-lg border border-transparent bg-sky-600 text-white hover:bg-sky-700 disabled:opacity-50 disabled:pointer-events-none dark:focus:outline-none dark:focus:ring-1 dark:focus:ring-gray-600";
        
        public const string LargeDangerButtonFullWidth = LargeDangerButton + "w-full";
        public const string LargeDangerButton = "no-underline py-3 px-4 inline-flex justify-center items-center gap-x-2 text-sm font-semibold rounded-lg border border-transparent bg-red-600 text-white hover:bg-red-700 disabled:opacity-50 disabled:pointer-events-none dark:focus:outline-none dark:focus:ring-1 dark:focus:ring-gray-600";

        public const string TextInputLabel = "block text-sm mb-2 dark:text-white";
        public const string TextInputLabelSideAction = "text-sm mb-2 text-sky-600 decoration-2 hover:underline font-medium dark:focus:outline-none dark:focus:ring-1 dark:focus:ring-gray-600";
        public const string TextInputField = "py-3 px-4 block w-full border border-gray-200 rounded-lg text-sm focus:border-sky-500 focus:ring-sky-500 disabled:opacity-50 disabled:pointer-events-none dark:bg-slate-900 dark:border-gray-700 dark:text-gray-400 dark:focus:ring-gray-600";
        public const string ValidationSummary = "py-3 px-4 mb-6 border border-red-600 bg-red-50 rounded-lg " + ErrorMessage;
        public const string ErrorMessage = "text-sm text-red-600 mt-2";
    }
}
