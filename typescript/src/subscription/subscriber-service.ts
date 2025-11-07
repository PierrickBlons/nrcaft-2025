import { Injectable } from '@nestjs/common'
import { Subscriber } from '@prisma/client'
import { v4 } from 'uuid'

export interface ISubscriberService {
  createSubscriberAsync(
    email: string,
    name: string,
    websiteUri: string,
  ): Promise<Subscriber>
}

@Injectable()
export class SubscriberService implements ISubscriberService {
  async createSubscriberAsync(
    email: string,
    name: string,
    websiteUri: string,
  ): Promise<Subscriber> {
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

    return {
      email,
      name,
      websiteUri,
      id: v4(),
      registeredAt: new Date(),
    } as Subscriber
  }
}
