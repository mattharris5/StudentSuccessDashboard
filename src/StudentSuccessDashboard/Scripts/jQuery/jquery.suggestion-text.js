/**
* A small plugin that adds a helper text to field.
*
* When the field receives focus (click, tabbed into etc), the helper text
* is removed.
*
* When the field looses focus again, the helper text will be set in the field
* again, IF the value is the same as the helper text OR the field is empty.
*
* Get updates, news and post bugs on:
* https://github.com/jimmiw/jquery-suggestion-text
*
* @author jimmiw (Jimmi Westerberg)
* @homepage http://westsworld.dk
* @since 2011-02-16
* @version 1.0
*/
(function($) {
  // jQuery plugin definition
  $.fn.suggest = function(options) {
    // the default options for the plugin.
    var defaultOptions = {
      cssClass: 'blurry',
      text: 'My help text'
    };
    // merges the options
    var options = $.extend(defaultOptions, options);
  
    // runs through each element this function is called on
    // e.g. with $('input').suggest({}) we would have multiple objects.
    return this.each(function() {
      // fetches and initializes the current element.
      var obj = $(this);
    
      // if the object has no value, then set the suggestion and add the css class
      if(obj.val() == "") {
        // sets the default suggestion text and the
        obj.val(options.text).addClass(options.cssClass);
      }
    
      // when the field recieves focus
      obj.focus(function() {
        // if the value is the same as we "set it" to
        if(obj.val() == options.text) {
          // resets the value in the field
          obj.val("");
        }
        // removes the blurry class
        obj.removeClass(options.cssClass);
      });
    
      // when the user leaves the field
      obj.blur(function() {
        if(obj.val() == options.text || obj.val() == '') {
          // adds the suggestion
          obj.val(options.text);
          // adds the css class
          obj.addClass(options.cssClass);
        }
      });
    });
    
    // allows for jQuery chaining
    return this;
  };
})(jQuery);