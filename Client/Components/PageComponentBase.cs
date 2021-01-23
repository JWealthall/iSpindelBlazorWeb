using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace iSpindelBlazorWeb.Client.Components
{
    public class PageComponentBase : ComponentBase
    {
        [Inject]
        protected NavigationManager NavManager { get; set; }
        [Inject]
        protected PageHistoryState PageState { get; set; }
        
        public PageComponentBase() { }
        public PageComponentBase(NavigationManager navManager, PageHistoryState pageState)
        {
            NavManager = navManager;
            PageState = pageState;
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            PageState.AddPageToHistory(NavManager.Uri);
        }
    }

    // TODO:
    //  - Add a method to "Go Back" (which can be added as an "on click"
    //  - "Pop" the history(ies) when going back
    //  - Only add a history if not the same as the last one
    //  - Simple reset method (for example on the home page)
    //  - Persist the information so that even a refresh will still work
    
    // NOTE:
    //  - This could be achieved by creating a global "Go To Previous" Interop call
    public class PageHistoryState
    {
        private List<string> previousPages;

        public PageHistoryState()
        {
            previousPages = new List<string>();
        }
        
        public void AddPageToHistory(string pageName)
        {
            if (previousPages.LastOrDefault() == pageName) return;
            previousPages.Add(pageName);
        }

        public string GetGoBackPage()
        {
            if (previousPages.Count > 1)
            {
                // You add a page on initialization, so you need to return the 2nd from the last
                return previousPages.ElementAt(previousPages.Count - 2);
            }

            // Can't go back because you didn't navigate enough
            return previousPages.FirstOrDefault();
        }

        public bool CanGoBack()
        {
            return previousPages.Count > 1;
        }
    }
}
