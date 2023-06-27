let authInterval = null

const authApi = axios.create({
  baseURL: config.infowijsApiEndpoint,
  headers: {
    'X-Infowijs-Client': `nl.infowijs.schoolwiki.web/${config.district}`
  }
})

function initAuth () {
  if ($('.auth-qr').length) {
    $('#auth-toggle-email').click(() => authToggle('email'))
    $('#auth-toggle-qr').click(() => authToggle('qr'))
    $('#auth-login-email').submit(authEmail)
    $('#auth-reset').click(() => {
      if (authInterval) {
        clearInterval(authInterval)
      }
      $('#auth-login-email').hide()
      $('#auth-polling').show()
    })
    authLoadQR()
  }
}

function authToggle (mode) {
  $('.auth-email').toggle(mode === 'email')
  $('.auth-qr').toggle(mode === 'qr')
  if (mode === 'email') {
    if (authInterval) {
      clearInterval(authInterval)
    }
    $('#login-email').focus()
  } else {
    authLoadQR()
  }
}

async function saveSession (refreshToken) {
  // Create a transfer token
  const { data: { data: transferToken } } = await authApi.post(`/sessions/transfer`)
  await authApi.post(`/sessions/transfer/${transferToken}`, {
    refreshToken
  })

  location.href = `?hoy=${transferToken}`
}

async function authPollSession (session) {
  try {
    const { data } = await authApi.post(`/sessions/${session.id}/${session.customer_product_id}/${session.user_id}`)
    if (data.data && typeof data.data === 'string') {
      saveSession(data.data)
    }
  } catch (e) {
    location.reload()
  }
}

async function authPollTransferSession (id) {
  try {
    const { data } = await authApi.get(`/sessions/transfer/${id}`)
    if (data.data && typeof data.data === 'string') {
      saveSession(data.data)
    }
  } catch (e) {
    location.reload()
  }
}

async function authEmail (e) {
  e.preventDefault()
  const username = $('#login-email').val()

  // Initiate session
  try {
    // Get customers
    const { data: customers } = await authApi.post(`/sessions/customer-products`, { username })
    if (!customers || !customers.data || !customers.data.length) {
      alert('Sorry, dat e-mailadres is niet bekend.')
      return
    }

    // Find customer that is connected to this community
    const customer = customers.data.find(({ name }) => config.hoyDomain.split(',').includes(name))
    if (!customer) {
      alert('Sorry, dit e-mailadres heeft geen toegang tot deze Hoy omgeving.')
      return
    }

    // Create session
    const { data: session } = await authApi.post(`/sessions`, { username, communityName: customer.name })
    if (authInterval) {
      clearInterval(authInterval)
    }
    authInterval = setInterval(() => authPollSession(session.data), 2000)
    $('#auth-login-email').hide()
    $('.auth-polling').show()
  } catch (e) {
    alert('Sorry, dat e-mailadres is niet bekend of heeft geen toegang tot deze Hoy omgeving.')
  }
}

$(document).on('pjax:end', initAuth)
initAuth()

async function authLoadQR () {
  const { data } = await authApi.post(`/sessions/transfer`)
  if (authInterval) {
    clearInterval(authInterval)
  }

  $('.auth-qr .qr-container').empty();
  new QRCode($('.auth-qr .qr-container')[0], {
    text: `hoy_scan://v1/login/${data.data}`,
    width: 200,
    height: 200,
    correctLevel : QRCode.CorrectLevel.H
  });

  authInterval = setInterval(() => authPollTransferSession(data.data), 2000)
}
