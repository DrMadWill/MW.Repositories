# MW.Saga.MassTransit və MW.Messaging.MassTransit Sərhədi

## Örtüşmə Sahələri

| Sahə | MW.Messaging.MassTransit | MW.Saga.MassTransit |
|---|---|---|
| Retry | Mesaj istehlakı üçün retry siyasəti | Saga event handling üçün retry siyasəti |
| Observability | Mesaj publish/consume logging | Saga lifecycle logging |
| Endpoint Naming | `ServiceEndpointNameFormatter` | `SagaEndpointNameFormatter` (eyni pattern) |
| Headers | Mesaj header enrichment/propagation | Saga kontekst header oxuma |
| Correlation | Mesaj korrelyasiya konteksti | Saga korrelyasiya resolver adapter |
| Context | `IMessageContextAccessor`/`IMessageExecutionContext` | `ISagaContextAccessor`/`ISagaExecutionContext` |
| ActivitySource | "MW.Messaging" | "MW.Saga" |

## Yenidən İstifadə Nöqtələri

- Endpoint naming pattern (`KebabCaseEndpointNameFormatter` base class) hər iki paketdə eyni şəkildə istifadə olunur
- Header constant adları (`X-Source-Service`) ortaq messaging abstractions-dan gəlir
- Retry konfiqurasiya pattern-i eynidir (interval əsaslı retry)

## Ayrı Qalan Hissələr

- Saga lifecycle observability saga abstractions-a əsaslanır (`ISagaObserver`, `SagaObservabilityContext`)
- Messaging observability messaging abstractions-a əsaslanır (`MessageLogContext`, `IPublishObserver`)
- Saga kontekst yayılması saga-spesifik filter ilə həyata keçirilir
- Messaging kontekst yayılması ayrı consume filter ilə həyata keçirilir

## Tövsiyə

- İnfrastruktur helper-ləri dublikat etməkdənsə, ortaq pattern-ləri abstract base class və ya utility vasitəsilə paylaşmaq tövsiyə olunur
- Hər iki paket eyni servisdə istifadə edilə bilər — onlar müstəqil və bir-birini tamamlayır
