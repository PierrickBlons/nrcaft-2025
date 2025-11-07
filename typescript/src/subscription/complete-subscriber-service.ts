import { Inject, Injectable } from '@nestjs/common'
import { Subscriber } from '@prisma/client'
import { PrismaService } from '../shared/infrastructure/sql/prisma-service'
import { ISubscriberService } from './subscriber-service'

@Injectable()
export class CompleteSubscriberService implements ISubscriberService {
  constructor(@Inject(PrismaService) readonly prisma: PrismaService) {}

  async createSubscriberAsync(
    email: string,
    name: string,
    websiteUri: string,
  ): Promise<Subscriber> {
    // Check if subscriber already exists
    const existingSubscriber = await this.prisma.subscriber.findUnique({
      where: { email },
    })

    if (existingSubscriber) {
      return existingSubscriber
    }

    const domain = new URL(websiteUri).hostname
    const subscriptionDetailsResponse = await fetch(
      `https://api.subscriptions.internal/subscribers?domain=${encodeURIComponent(
        domain,
      )}`,
    )

    if (!subscriptionDetailsResponse.ok) {
      throw new Error(
        `Failed to fetch subscription details for domain: ${domain}`,
      )
    }

    const details = (await subscriptionDetailsResponse.json()) as {
      status: string
    }
    if (!details || details.status !== 'active') {
      throw new Error(`Subscription status is not active for domain: ${domain}`)
    }

    // Create new subscriber
    const subscriber = await this.prisma.subscriber.create({
      data: {
        email,
        name,
        websiteUri,
        registeredAt: new Date(),
      },
    })

    return subscriber
  }
}
