import { Injectable } from '@nestjs/common'

@Injectable()
export class EmailValidationService {
  baseUrl: string = 'https://api.emailvalidation.com'

  public async isEmailSafe(email: string): Promise<boolean> {
    try {
      console.info('Validating email safety for: %s', email)
      const response = await fetch(
        `${this.baseUrl}/validate?email=${encodeURIComponent(email)}`,
      )

      if (response.ok) {
        const content = await response.text()
        console.info('Email validation response: %s', content)

        return content.toLowerCase().includes('"safe":true')
      }

      console.warn(
        'Email validation service returned status: %d',
        response.status,
      )
      return false
    } catch (error) {
      console.error(
        'Error validating email safety for: %s',
        error instanceof Error ? error : new Error(String(error)),
        email,
      )
      return false
    }
  }
}
