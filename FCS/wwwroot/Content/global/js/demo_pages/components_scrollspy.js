/* ------------------------------------------------------------------------------
 *
 *  # Scrollspy
 *
 *  Demo JS code for components_scrollspy.html page
 *
 * ---------------------------------------------------------------------------- */


// Setup module
// ------------------------------

var Scrollspy = function () {


    //
    // Setup module components
    //

	// Sticky
    var _componentSticky = function() {
        if (!$().stick_in_parent) {
            console.warn('Warning - sticky.min.js is not loaded.');
            return;
        }

        // Initialize
        $('.sidebar-sticky .fixedcontrolbar').stick_in_parent({
            offset_top: 50,
            parent: '.content'
        });

        // Detach on mobiles
        $('.sidebar-mobile-component-toggle').on('click', function() {
            $('.sidebar-sticky .fixedcontrolbar').trigger("sticky_kit:detach");
        });
    };


    //
    // Return objects assigned to module
    //

    return {
        initComponents: function() {
            _componentSticky();
        }
    }
}();


// Initialize module
// ------------------------------

document.addEventListener('DOMContentLoaded', function() {
    Scrollspy.initComponents();
});
