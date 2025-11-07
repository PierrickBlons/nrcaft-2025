import { Module } from '@nestjs/common'
import { SubscriberController } from './subscription/subscriber-controller'

@Module({
  imports: [],
  controllers: [SubscriberController],
  providers: [SubscriberController],
})
export class AppModule {}
