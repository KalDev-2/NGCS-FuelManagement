/* ------------------------------------------------------------------------------
 *
 *  # Internationalization - change language without page reload
 *
 *  Demo JS code for internationalization_switch_direct.html page
 *
 * ---------------------------------------------------------------------------- */


// Setup module
// ------------------------------

var I18nextDirect = function() {


    //
    // Setup module components
    //

    // Change language without page reload
    var _componentI18nextDirect = function() {
        if (typeof i18next == 'undefined') {
            console.warn('Warning - i18next.min.js is not loaded.');
            return;
        }


        // Configuration
        // -------------------------

        // Define main elements
        var $switchContainer = $('.language-switch'),
            englishLangClass = '.english',
            amharicLangClass = '.amharic',
            $localizationElement = $('body');

        // Add options
        i18next.use(i18nextXHRBackend).use(i18nextBrowserLanguageDetector).init({
            backend: {
                loadPath: '../../../Content/global/locales/{{lng}}.json'
            },
            debug: true,
            fallbackLng: false
        },
        function (err, t) {
            
            // Initialize library
            jqueryI18next.init(i18next, $);

            // Initialize translation
            $localizationElement.localize();

            // To avoid FOUC when translation gets initialized,
            // use data-fouc attribute in all elements by default. When translation
            // is initialized, remove it from all elements
            $localizationElement.find('[data-i18n]').removeAttr('data-fouc');
        });


        // Change languages in dropdown
        // -------------------------

        // Do stuff when i18Next is initialized
        i18next.on('initialized', function() {

            // English
            if(i18next.language === "en") {

                // Set active class
                $('.navbar-nav-link' + englishLangClass).parent().addClass('active');
            }

            // Amharic
            if(i18next.language === "am") {

                // Set active class
                $('.navbar-nav-link' + amharicLangClass).parent().addClass('active');
            }

           
        });


        // Change languages in navbar
        // -------------------------

        // English
        $(englishLangClass).on('click', function () {

            // Change language
            i18next.changeLanguage('en');

            // When changed, run translation again
            i18next.on('languageChanged', function() {
                $localizationElement.localize();
            });

            // Set active class
            $switchContainer.find('.dropdown-item.active, .nav-item.active').removeClass('active');
            $('.navbar-nav-link' + englishLangClass).parent().addClass('active');
        });

        // Amharic
        $(amharicLangClass).on('click', function () {

            // Change language
            i18next.changeLanguage('am');

            // When changed, run translation again
            i18next.on('languageChanged', function() {
                $localizationElement.localize();
            });

          
            
            // Set active class
            $switchContainer.find('.dropdown-item.active, .nav-item.active').removeClass('active');
            $('.navbar-nav-link' + amharicLangClass).parent().addClass('active');
        });
    };
    //
    // Return objects assigned to module
    //

    return {
        init: function() {
            _componentI18nextDirect();
        }
    }
}();


// Initialize module
// ------------------------------

document.addEventListener('DOMContentLoaded', function() {
    I18nextDirect.init();
});
