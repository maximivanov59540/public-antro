# QA Final Check

Дата проверки: 2026-05-23

Проверка выполнена как статический QA-проход по коду и контенту без запуска `dotnet build` и `dotnet test`.

## Passed checks

- Репозиторный минимум для MVP соблюдён: файл `.gitignore` существует и включает `bin/`, `obj/`, `*.log`, `TestResults/`, `coverage/`.
- В рабочем дереве не найдено `*.log` файлов.
- По статическому поиску в `src/` и `tests/` не обнаружены backend/API/controller wiring, EF Core, database migrations, ASP.NET Identity, auth/account system, `localStorage`/cookie persistence, analytics/tracking scripts и admin/CMS код.
- Упоминания `mos.ru`, `Госуслуги` и `СФР` встречаются как контентные/legal source ссылки и suggested routes в typed catalog, а не как внешние runtime integration points.
- Региональная модель не расширена за пределы Moscow 2026 catalog.
- Маршруты по чеклисту присутствуют в Razor-страницах: `/`, `/stage/newborn` через шаблон `/stage/{StageSlug}`, `/questionnaire`, `/dashboard`, `/benefits/{slug}`, `/demo/maria`.
- Слаг референсной меры приведён к ожидаемому маршруту `young-family-payment`, поэтому путь `/benefits/young-family-payment` соответствует каталогу.
- Нормальный сценарий анкета → dashboard сохранён: профиль читается из `InMemoryUserProfileState`, а dashboard строится через `DashboardBuilder`.
- Demo route не хардкодит результаты: `/demo/maria` использует `DemoProfiles.MariaNewborn(DateOnly today)`, typed catalog Moscow 2026, обычный evaluator/sorter и `DashboardBuilder`.
- Архитектурный инвариант соблюдён: eligibility, deadline, amount и sorting живут в application-слое, а не в Razor.
- UI потребляет view models и builder outputs: dashboard/detail страницы не создают локальные массивы льгот и не считают суммы/сроки в шаблоне.
- Typed catalog используется как источник контента: льготы описаны в `Moscow2026BenefitCatalog*`, а не в Razor.
- По статической проверке каталог остаётся MVP-фиксированным на 31 льготу; в `Moscow2026BenefitCatalog.All` собрана одна typed-коллекция без регионального расширения.
- В каталоге есть покрытие на базовую схему: уникальность id/slug, непустые copy/amount/stage/documents/action steps/legal sources и ровно 31 benefit описаны существующими тестами каталога.
- Ключевые сроки в каталоге заданы ожидаемыми правилами:
  Gift Set: `2` месяца от рождения.
  Federal birth grant: `6` месяцев от рождения.
  Moscow birth payment: `6` месяцев от рождения.
  Young family payment: `12` месяцев от рождения.
  Feeding breaks: до `1.5` лет ребёнка.
  Child care leave: до `3` лет ребёнка.
  Parent dismissal protection: до `3` лет ребёнка.
- Денежная модель выглядит корректно:
  exact amounts представлены typed money values и могут участвовать в totals;
  `UpTo` и `Range` не суммируются как точные значения;
  natural support и status-only rights не считаются деньгами;
  percentage subsidies не переводятся в фиктивные рублёвые суммы.
- Dashboard не заявляет ложную точность: summary отделяет точные суммы от range/up-to/non-monetary cases.
- Unavailable benefits и `NeedsMoreInfo` состояния выводятся с объясняющим reason/summary text, а не скрываются и не отвергаются молча.
- Copy/privacy базово соблюдены:
  landing содержит дисклеймер про граждан РФ и регистрацию в Москве;
  questionnaire явно говорит, что в демо-режиме ответы не сохраняются и не отправляются на сервер.
- В ходе M15-прохода выполнены точечные правки без расширения scope:
  добавлен `*.log` в `.gitignore`;
  унифицирован слаг `young-family-payment`;
  локализованы общие evaluator/deadline/amount/draft-content строки, чтобы не вытекал английский template residue в UI.

## Warnings

- В каталоге по-прежнему есть `Draft` и `NeedsReview` контент. Это допустимо для MVP, но часть юридических формулировок и practical notes ещё требует редакторской проверки.
- В части catalog notes и source metadata остаются англоязычные или полуслужебные формулировки уровня MVP, особенно вне полированного кейса `young-family-payment`.
- В проекте физически присутствуют существующие `bin/` и `obj/` директории. Они не изменялись в этом проходе, но их tracked/untracked статус нельзя подтвердить без git metadata.

## Known limitations

- В рабочей папке нет директории `.git`, поэтому нельзя достоверно подтвердить, что generated artifacts не tracked. Проверка ограничилась наличием `.gitignore` и поиском файлов на диске.
- Этот QA-проход не включал `dotnet restore`, `dotnet build` и `dotnet test` по прямому ограничению milestone.
- Каталог остаётся демонстрационным контентом по Москве 2026 и не претендует на production-grade legal verification.
- Demo profile использует ближайшие доступные enum-значения MVP, потому что текущая доменная модель грубее, чем narrative-описание сценария Марии.

## Recommended manual verification commands

- `dotnet restore Antro.sln`
- `dotnet build Antro.sln`
- `dotnet test Antro.sln`

## Manual demo route checklist

- Открыть `/demo/maria`.
- Убедиться, что видна заметка: `Демо-сценарий: Мария, Москва, первый ребёнок, 14 дней.`
- Проверить, что dashboard заполнен не фейковыми карточками, а результатом обычного builder path.
- Открыть карточку `Выплата молодой семье` и проверить маршрут `/benefits/young-family-payment`.
- На detail page проверить наличие status pill, amount, deadline, conditions, documents, action steps, legal sources и back link.
- Вернуться на `/dashboard` и убедиться, что normal questionnaire path не сломан.
- Пройти `/questionnaire`, сформировать профиль вручную и проверить, что dashboard строится без demo-only логики.
- Сравнить даты и дедлайны в demo scenario с выбранной стратегией clock: ребёнок должен быть на 14 дней младше текущей app date.
