import { http, HttpResponse, passthrough } from 'msw'

const localhostHandler = http.all('http://127.0.0.1*', () => passthrough())

const emailValidateHandler = http.get(
  'https://api.emailvalidation.com/validate',
  ({ request }) => {
    const url = new URL(request.url)
    const email = url.searchParams.get('email') || ''

    return HttpResponse.json({ email, safe: true })
  },
)

const subscriberHandler = http.get(
  'https://api.subscriptions.internal/subscribers',
  ({ request }) => {
    const url = new URL(request.url)
    const domain = url.searchParams.get('domain') || ''

    return HttpResponse.json({ domain, status: 'active' })
  },
)

export const DEFAULT_HANDLERS = [
  localhostHandler,
  emailValidateHandler,
  subscriberHandler,
]

export const emailValidationHandler = (body: {
  email: string
  safe: boolean
}) =>
  http.get('https://api.emailvalidation.com/validate', () => {
    return HttpResponse.json(body)
  })
