//THIS IS CODE FOR A CLOUDFLARE WORKER!

addEventListener("fetch", event => {
    event.respondWith(handleRequest(event.request));
});

async function handleRequest(request) {
    // Get the URL from the query string, default to an empty string if not provided
    const { searchParams } = new URL(request.url);
    const username = searchParams.get("username") || "";
    const password = searchParams.get("password") || "";
    const schoolUUID = searchParams.get("schoolUUID") || "";
    
    // Check if the username, password, and schoolUUID are provided
    if (!username || !password || !schoolUUID) {
        return new Response(`Missing required parameters:${username ? "" : " username" }${ password ? "" : " password" }${ schoolUUID ? "" : " schoolUUID" }`, { status: 400 });
    }
    
    //GENREATE A CODE CHALLENGE AND CODE VERIFIER
    const code_verifier = generateCodeVerifier();
    const code_challenge = await generateCodeChallenge(code_verifier);
    
    let baseUrl = "https://inloggen.somtoday.nl/oauth2/authorize?redirect_uri=somtoday://nl.topicus.somtoday.leerling/oauth/callback&client_id=somtoday-leerling-native&response_type=code&state={0}&scope=openid&tenant_uuid={1}&session=no_session&code_challenge={2}&code_challenge_method=S256&knf_entree_notification"
    .replace("{0}", randomString(8))
    .replace("{1}", schoolUUID)
    .replace("{2}", code_challenge);
    
    //SEND THE URL TO THE CLIENT WITHOUT FOLLOWING THE REDIRECT
    const response = await fetch(baseUrl, {
        redirect: "manual",
        headers: {
            "Cookie": "production-authenticator-stickiness=1"
        },
        mode: "no-cors"
    });

    // Check if the response is a redirect (302)
    if (response.status !== 302) {
        return new Response("No redirect found", { status: 200 });
    }
    
    const locationHeader = response.headers.get("Location").toString();
    const authCode = locationHeader.substring(6); //the query string where the first 6 chars are removed (?auth=)
    //get the production-authenticator-stickiness cookie from Set-Cookie header
    const setCookie = response.headers.getSetCookie();
    const cookie = setCookie.find((cookie) => cookie.name === "production-authenticator-stickiness");
    
    if (!cookie) {
        return new Response("No Pr. auth cookie found", { status: 200 });
    }
    
    //TEST IF EVERYTHING IS STILL WORKING
    return new Response(`Username: ${username} Password: ${password} SchoolUUID: ${schoolUUID} CodeVerifier: ${code_verifier} CodeChallenge: ${code_challenge} AuthCode: ${authCode} Cookie: ${cookie.value}`, { status: 200 });
}


//UTIL FUNCTIONS
function generateCodeVerifier() {
    const array = new Uint8Array(32);
    crypto.getRandomValues(array);
    return btoa(String.fromCharCode.apply(null, new Uint8Array(array))).replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
}

async function generateCodeChallenge(code_verifier) {
    return btoa(String.fromCharCode.apply(null, new Uint8Array(new Uint8Array(await crypto.subtle.digest("SHA-256", new TextEncoder().encode(code_verifier)))))).replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
}

const randomString = (length) => { 
    let result = ''; 
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789'; 
    const charactersLength = characters.length; 
    for (let i = 0; i < length; i++) { 
        result += characters.charAt(Math.floor(Math.random() * charactersLength)); 
    } 
    return result; 
}