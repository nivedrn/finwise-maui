namespace finwise.maui.Services
{
    /// <summary>
    /// Navigation used by view models, so they never create pages or touch Shell directly.
    /// </summary>
    public interface INavigationService
    {
        Task OpenSearchAsync();

        Task OpenAddActivityAsync();

        /// <summary>Closes the top-most modal page, if any is open.</summary>
        Task CloseModalAsync();
    }
}
