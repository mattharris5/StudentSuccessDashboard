/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
/// <reference path="../../typings/jquery/jquery.d.ts" />

$("#imgUserManagement").click(function (event) {
    event.preventDefault();
    $("#screenshotModal").modal({ show: true });
});
$("#imgData").click(function (event) {
    event.preventDefault();
    $("#screenshotDataModal").modal({ show: true });
});

var identityProviders = [];
var cookieName = "ACSChosenIdentityProvider-20173602";
var cookieExpiration = 30; // days
var maxImageWidth = 240;
var maxImageHeight = 40;

// This function will be called back by the HRD metadata, and is responsible for displaying the sign-in page.
function ShowSigninPage(json) {
    var cookieName = GetHRDCookieName();
    var numIdentityProviderButtons = 0;
    var showMoreProvidersLink = false;
    var loginContentDiv = document.getElementById("loginContent");
    identityProviders = json;

    if (identityProviders.length == 0) {
        loginContentDiv.appendChild(document.createElement("br"));
        loginContentDiv.appendChild(document.createTextNode("Error: No identity providers are associated with this application."));
    }

    // Loop through the identity providers
    for (var i in identityProviders) {
        if (identityProviders[i].Name) {
            // Show all sign-in options if no cookie is set
            if (cookieName === null) {
                if (!(identityProviders[i].EmailAddressSuffixes.length > 0)) {
                    // Only show a button if there is no email address configured for this identity provider.
                    CreateIdentityProviderButton(identityProviders[i]);
                    numIdentityProviderButtons++;
                }
            }
            // Show only the last selected identity provider if a cookie is set
            else {
                if (cookieName == identityProviders[i].Name) {
                    CreateIdentityProviderButton(identityProviders[i]);
                    numIdentityProviderButtons++;
                }
                else {
                    showMoreProvidersLink = true;
                }
            }
        }
    }
    //If the user had a cookie but it didn't match any current identity providers, show all sign-in options 
    if (cookieName !== null && numIdentityProviderButtons === 0) {
        ShowDefaultSigninPage();
    }
    //Othewise, render the sign-in page normally
    else {
        ShowSigninControls(numIdentityProviderButtons, showMoreProvidersLink);
    }

    loginContentDiv.style.display = "";
}

// Resets the sign-in page to its original state before the user logged in and received a cookie.
function ShowDefaultSigninPage() {
    var numIdentityProviderButtons = 0;
    document.getElementById("identityProvidersList").innerHTML = "";
    for (var i in identityProviders) {
        if (identityProviders[i].Name) {
            if (!(identityProviders[i].EmailAddressSuffixes.length > 0)) {
                CreateIdentityProviderButton(identityProviders[i]);
                numIdentityProviderButtons++;
            }
        }
    }
    ShowSigninControls(numIdentityProviderButtons, false);
}

//Reveals the sign-in controls on the sign-in page, and ensures they are sized correctly
function ShowSigninControls(numIdentityProviderButtons, showMoreProvidersLink) {
    //Display the identity provider links, and size the page accordingly
    if (numIdentityProviderButtons > 0) {
        document.getElementById("loginControls").style.display = "";
        if (numIdentityProviderButtons > 4) {
            var height = 325 + ((numIdentityProviderButtons - 4) * 55);
            document.getElementById("loginContent").style.height = height + "px";
        }
    }
    //Show a link to redisplay all sign-in options
    if (showMoreProvidersLink) {
        document.getElementById("moreProvidersLink").style.display = "";
    }
    else {
        document.getElementById("moreProvidersLink").style.display = "none";
    }
}

//Creates a stylized link to an identity provider's login page
function CreateIdentityProviderButton(identityProvider) {
    var idpList = document.getElementById("identityProvidersList");
    var liButton = document.createElement("li");
    var button = document.createElement("button");
    button.setAttribute("name", identityProvider.Name);
    button.setAttribute("id", identityProvider.LoginUrl);
    button.className = "IdentityProvider btn btn-primary btn-small";
    button.onclick = IdentityProviderButtonClicked;
    
    // Display an image if an image URL is present
    if (identityProvider.ImageUrl.length > 0) {
        
        var img = document.createElement("img");
        img.className = "IdentityProviderImage";
        img.setAttribute("src", identityProvider.ImageUrl);
        img.setAttribute("alt", identityProvider.Name);
        img.setAttribute("border", "0");
        img.setAttribute("onload", "ResizeImage(img)");
        
        button.appendChild(img);
    }
    // Otherwise, display a text link if no image URL is present
    else if (identityProvider.ImageUrl.length === 0) {
        button.appendChild(document.createTextNode(identityProvider.Name));
    }
    
    liButton.appendChild(button);
    idpList.appendChild(liButton);
}

// Gets the name of the remembered identity provider in the cookie, or null if there isn't one.
function GetHRDCookieName() {
    var cookie = document.cookie;
    if (cookie.length > 0) {
        var cookieStart = cookie.indexOf(cookieName + "=");
        if (cookieStart >= 0) {
            cookieStart += cookieName.length + 1;
            var cookieEnd = cookie.indexOf(";", cookieStart);
            if (cookieEnd == -1) {
                cookieEnd = cookie.length;
            }
            return decodeURIComponent(cookie.substring(cookieStart, cookieEnd));
        }
    }
    return null;
}

// Sets a cookie with a given name
function SetCookie(name) {
    var expiration = new Date();
    expiration.setDate(expiration.getDate() + cookieExpiration);
    var secure = "";

    // If your application uses SSL, consider setting secure=";secure".
    document.cookie = cookieName + "=" + encodeURIComponent(name) + ";expires=" + expiration.toUTCString() + secure;
}

// Sets a cookie to remember the chosen identity provider and navigates to it.
function IdentityProviderButtonClicked() {
    SetCookie(this.getAttribute("name"));
    var identityProviderLocation = this.getAttribute("id");
    var identityProviderLocationQueryString = encodeURIComponent(identityProviderLocation);
    window.location.href = "/Account/LogOn/?federationLocation=" + identityProviderLocationQueryString;
    return false;
}

// If the image is larger than the button, scale maintaining aspect ratio.
function ResizeImage(img) {
    if (img.height > maxImageHeight || img.width > maxImageWidth) {
        var resizeRatio = 1;
        if (img.width / img.height > maxImageWidth / maxImageHeight)
        {
            // Aspect ratio wider than the button
            resizeRatio = maxImageWidth / img.width;
        }
        else
        {
            // Aspect ratio taller than or equal to the button
            resizeRatio = maxImageHeight / img.height;
        }
        
        img.setAttribute("height", img.height * resizeRatio);
        img.setAttribute("width", img.width * resizeRatio);
    }
}