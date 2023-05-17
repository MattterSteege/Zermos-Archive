addEventListener('fetch', event => {
    event.respondWith(handleRequest(event.request))
})

const CodeVerifier = 'ypeBEsobvcr6wjGzmiPcTaeG7_gUfE5yuYB3ha_uSLs'; //verifier
const CodeChallenge = 'DVYt9lHuu7e2HiDUDWl4YiyVzBGuvPmpS_mZYO7m10s'; //challenge

async function handleRequest(request) {

    if (request.method === 'OPTIONS') {
        return new Response(null, {
            headers: {
                'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Headers': 'Authorization, Range, Accept, Content-Type',
                'Access-Control-Allow-Methods': 'GET, OPTIONS',
                'Access-Control-Max-Age': '86400',
            },
        });
    }

    const { searchParams } = new URL(request.url);
    const username = searchParams.get('u') ?? '';
    const password = searchParams.get('p') ?? '';

    if (username === '' || password === '') {
        return new Response("you need to set the username and token password", {
            headers: {
                'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Headers': 'Authorization, Range, Accept, Content-Type',
                'Access-Control-Allow-Methods': 'GET, OPTIONS',
                'Access-Control-Max-Age': '86400',
            },
        });
    }

    //return new Response(username + " - " + password);

    const headers = new Headers({
        'Content-Type': 'application/x-www-form-urlencoded',
        'Origin': 'https://inloggen.somtoday.nl'
    });


    //step 1 - works
    let response = await fetch(`https://inloggen.somtoday.nl/oauth2/authorize?redirect_uri=somtodayleerling://oauth/callback&client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2&response_type=code&state=${generateRandomString(8)}&scope=openid&tenant_uuid=c23fbb99-be4b-4c11-bbf5-57e7fc4f4388&session=no_session&code_challenge=${CodeChallenge}&code_challenge_method=S256`, {
        redirect: "manual"
    });
    //why does this show status: pending?
    return new Response(response.status + " - " + response.statusText);

    let LocationHeader = response.headers.get("Location");
    let myUrl = new URL(LocationHeader);
    let authToken = myUrl.searchParams.get("auth");

    if (!authToken) return new Response("failed to get authToken");



    //step 2 - works
    var urlencoded = new URLSearchParams();
    urlencoded.append("loginLink", "x");
    urlencoded.append("usernameFieldPanel:usernameFieldPanel_body:usernameField", username);

    response = await fetch(`https://inloggen.somtoday.nl/?-1.-panel-signInForm&auth=${authToken}`, {
        method: 'POST',
        headers: headers,
        body: urlencoded,
        redirect: 'follow'


    }).then(function(reponse) { response.sendStatus(204); });



    urlencoded = new URLSearchParams();
    urlencoded.append("loginLink", "x");
    urlencoded.append("passwordFieldPanel:passwordFieldPanel_body:passwordField", username);

    requestOptions = {
        method: 'POST',
        headers: headers,
        body: urlencoded,
        redirect: 'manual'
    };

    response = fetch(`https://inloggen.somtoday.nl/login?1-1.-passwordForm&auth=${authToken}`, requestOptions).then(function(reponse) { response.sendStatus(204);});





    // urlencoded = new URLSearchParams();
    // urlencoded.append("loginLink", "x");
    // urlencoded.append("passwordFieldPanel:passwordFieldPanel_body:passwordField", username);

    // requestOptions = {
    // method: 'POST',
    // headers: headers,
    // body: urlencoded,
    // redirect: 'manual'
    // };

    // response = fetch(`https://inloggen.somtoday.nl/login?1-1.-passwordForm&auth=${authToken}`, requestOptions);

    //TODO: find why this doesn't return a 302... fucking stupid

    LocationHeader = await response.headers?.get("Location") ?? 'https://placeholder.com?code=error';
    myUrl = new URL(LocationHeader);
    authCode = myUrl.searchParams.get("code");

    if (authCode === 'error') return new Response("failed to get authCode - " + await response.status);

    return new Response(authCode)
}

const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
const charsLength = chars.length;

function generateRandomString(count) {
    let stringChars = "";
    while (count-- >= 0) {
        stringChars += chars.charCodeAt((Math.random() * charsLength) | 0);
    }
    return stringChars;
}