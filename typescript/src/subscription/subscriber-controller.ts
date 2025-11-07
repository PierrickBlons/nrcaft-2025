import {
  BadRequestException,
  Body,
  Controller,
  Inject,
  Post,
} from '@nestjs/common'
import { EmailValidationService } from './email-validation-service'
import type { ISubscriberService } from './subscriber-service'

@Controller()
export class SubscriberController {
  constructor(
    @Inject(EmailValidationService)
    readonly emailValidationService: EmailValidationService,
    @Inject('ISubscriberService')
    readonly subscriberService: ISubscriberService,
  ) {}

  @Post()
  async register(
    @Body() body: { name: string; email: string; websiteUri: string },
  ) {
    //todo validate body

    if (!(await this.emailValidationService.isEmailSafe(body.email))) {
      throw new BadRequestException('Unsafe email address')
    }

    // call service to create subscriber
    const {
      registeredAt: _registeredAt,
      updatedAt: _updatedAt,
      ...storedSubscriber
    } = await this.subscriberService.createSubscriberAsync(
      body.email,
      body.name,
      body.websiteUri,
    )

    // return http created with the subscriber information
    return storedSubscriber
  }
}
