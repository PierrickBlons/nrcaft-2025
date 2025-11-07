import { INestApplication } from '@nestjs/common'
import { Test, TestingModule } from '@nestjs/testing'
import request from 'supertest'
import { EmailValidationService } from './email-validation-service'
import { SubscriberController } from './subscriber-controller'
import { SubscriberService } from './subscriber-service'

describe('SubscriberController', () => {
  let app: INestApplication

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [SubscriberController],
      providers: [
        EmailValidationService,
        { provide: 'ISubscriberService', useClass: SubscriberService },
      ],
    }).compile()

    app = module.createNestApplication()
    await app.init()
  })

  afterEach(async () => {
    await app.close()
  })

  describe('POST /', () => {
    it('should register a new subscriber', async () => {
      const subscriberData = {
        name: 'test',
        email: 'test@example.com',
        websiteUri: 'https://example.com',
      }

      const response = await request(app.getHttpServer())
        .post('/')
        .send(subscriberData)

      expect(response.status).toBe(201)
      expect(response.body).toEqual({
        ...subscriberData,
        id: expect.any(String),
      })
    })
  })
})
