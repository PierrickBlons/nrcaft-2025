import { INestApplication } from '@nestjs/common'
import { Test, TestingModule } from '@nestjs/testing'
import { setupServer } from 'msw/node'
import request from 'supertest'
import { DEFAULT_HANDLERS } from '../../test/http-handlers/msw'
import { CompleteSubscriberService } from './complete-subscriber-service'
import { EmailValidationService } from './email-validation-service'
import { SubscriberController } from './subscriber-controller'

const server = setupServer(...DEFAULT_HANDLERS)

describe('SubscriberController', () => {
  let app: INestApplication

  beforeAll(() => server.listen())

  afterAll(() => server.close())

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [SubscriberController],
      providers: [
        EmailValidationService,
        { provide: 'ISubscriberService', useClass: CompleteSubscriberService },
      ],
    }).compile()

    app = module.createNestApplication()
    await app.init()
  })

  afterEach(async () => {
    server.resetHandlers()
    await app.close()
  })

  describe('POST /', () => {
    it('should register a new subscriber and return subscriber id', async () => {
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
        id: expect.any(String),
        email: 'test@example.com',
        name: 'test',
        websiteUri: 'https://example.com',
      })
    })

    it('should return existing subscriber if email already registered', async () => {
      const response = await request(app.getHttpServer()).post('/').send({
        name: 'test',
        email: 'existing@example.com',
        websiteUri: 'https://example.com',
      })

      expect(response.status).toBe(201)
      expect(response.body).toEqual({
        id: expect.any(String),
        email: 'existing@example.com',
        name: 'test',
        websiteUri: 'https://existing.com',
      })
    })
  })
})
