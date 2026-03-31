FROM rabbitmq:3-management

# Install the delayed message exchange plugin required by MassTransit's UseDelayedMessageScheduler()
ADD --chmod=644 https://github.com/rabbitmq/rabbitmq-delayed-message-exchange/releases/download/v3.13.0/rabbitmq_delayed_message_exchange-3.13.0.ez /plugins/
RUN rabbitmq-plugins enable --offline rabbitmq_delayed_message_exchange

